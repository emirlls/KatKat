using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.ResponseWrapping;

/// <summary>
/// Wraps every response under KatKat's own API route prefix (see
/// <see cref="KatKatRemoteServiceConsts.RoutePathPrefix"/>) in a single, predictable envelope -
/// { message, success, status, data } - regardless of whether the response came from a
/// controller action, an exception ABP already shaped as { error: {...} }, or an earlier
/// short-circuit (authentication, authorization, rate limiting). Everything outside that route
/// prefix (Swagger, the SignalR hub, static assets, other ABP module endpoints) passes through
/// untouched.
/// </summary>
public class ApiResponseWrapperMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static readonly PathString ApiRoutePathPrefix = "/" + KatKatRemoteServiceConsts.RoutePathPrefix;

    /// <summary>
    /// Error code prefixes whose message text is always safe/helpful to show a user as-is. Besides
    /// KatKat's own BusinessExceptions, ABP's AbpIdentityResultException (thrown by
    /// IdentityResult.CheckErrors() - "Username 'x' is already taken.", "Passwords must have at
    /// least one non alphanumeric character.", etc.) uses this "Volo.Abp.Identity:" prefix - those
    /// messages come directly from ASP.NET Core Identity's own validation, never from raw
    /// entity/id reflection, so they're exactly as safe as KatKat's own codes.
    /// </summary>
    private static readonly string[] TrustedErrorCodePrefixes = { KatKatErrorCodes.Prefix, "Volo.Abp.Identity:" };

    private readonly RequestDelegate _next;

    public ApiResponseWrapperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IStringLocalizer<KatKatResource> localizer)
    {
        if (!context.Request.Path.StartsWithSegments(ApiRoutePathPrefix))
        {
            await _next(context);
            return;
        }

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await _next(context);
        }
        finally
        {
            context.Response.Body = originalBody;
        }

        // Responses that must never carry a body (CORS preflight short-circuits to 204, etc.)
        // are passed through untouched - writing an envelope on top of them violates HTTP
        // semantics and Kestrel rejects it.
        if (IsBodyForbidden(context))
        {
            return;
        }

        var envelopeJson = BuildEnvelopeJson(context, buffer, localizer);

        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(envelopeJson);
        await context.Response.WriteAsync(envelopeJson);
    }

    private static bool IsBodyForbidden(HttpContext context)
    {
        if (HttpMethods.IsHead(context.Request.Method))
        {
            return true;
        }

        return context.Response.StatusCode is StatusCodes.Status204NoContent or StatusCodes.Status304NotModified;
    }

    /// <summary>
    /// Resolves and applies the culture UseAbpRequestLocalization negotiated for this request.
    /// That middleware restores the previous ambient culture as soon as its own next() call
    /// returns - which, from here, already happened (we run in the "after next()" half of our own
    /// InvokeAsync) - so CultureInfo.CurrentUICulture is back to the default by the time this runs.
    /// The negotiated culture is still readable from the request's IRequestCultureFeature, which
    /// (unlike the thread culture) isn't torn down, so we re-apply it just for this localizer call.
    /// </summary>
    private static string BuildEnvelopeJson(HttpContext context, MemoryStream buffer, IStringLocalizer<KatKatResource> localizer)
    {
        var requestCulture = context.Features.Get<IRequestCultureFeature>()?.RequestCulture;
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        if (requestCulture != null)
        {
            CultureInfo.CurrentCulture = requestCulture.Culture;
            CultureInfo.CurrentUICulture = requestCulture.UICulture;
        }

        try
        {
            return BuildEnvelopeJsonCore(context, buffer, localizer);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private static string BuildEnvelopeJsonCore(HttpContext context, MemoryStream buffer, IStringLocalizer<KatKatResource> localizer)
    {
        var statusCode = context.Response.StatusCode;
        var isSuccess = statusCode is >= 200 and < 300;

        var message = isSuccess
            ? localizer[ApiResponseConsts.DefaultSuccessMessageKey].Value
            : localizer[GetDefaultErrorMessageKey(statusCode)].Value;
        object? data = null;

        if (buffer.Length > 0)
        {
            buffer.Seek(0, SeekOrigin.Begin);
            using var document = JsonDocument.Parse(buffer);
            var root = document.RootElement;

            if (isSuccess)
            {
                data = root.Clone();
            }
            else if (root.TryGetProperty("error", out var error))
            {
                // Only two kinds of error message are safe to show a user: exceptions carrying one
                // of TrustedErrorCodePrefixes (always localized and GUID-free) and FluentValidation
                // failures (field-level detail already lives in validationErrors, the top-level
                // message is ABP's own generic "request is not valid" text). Anything else -
                // EntityNotFoundException, raw DB constraint violations, etc. - embeds technical
                // details (entity type names, raw ids) in its message, so we keep the generic
                // status-based fallback instead of leaking that text to the UI.
                var hasTrustedErrorCode = error.TryGetProperty("code", out var codeElement) &&
                    codeElement.ValueKind == JsonValueKind.String &&
                    Array.Exists(TrustedErrorCodePrefixes, prefix =>
                        (codeElement.GetString() ?? string.Empty).StartsWith(prefix, StringComparison.Ordinal));

                var hasValidationErrors = error.TryGetProperty("validationErrors", out var validationErrors) &&
                    validationErrors.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined);

                if ((hasTrustedErrorCode || hasValidationErrors) &&
                    error.TryGetProperty("message", out var messageElement) &&
                    messageElement.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(messageElement.GetString()))
                {
                    message = messageElement.GetString()!;
                }

                if (hasValidationErrors)
                {
                    data = validationErrors.Clone();
                }
            }
        }

        var envelope = new ApiResponse
        {
            Message = message,
            Success = isSuccess,
            Status = statusCode,
            Data = data
        };

        return JsonSerializer.Serialize(envelope, SerializerOptions);
    }

    /// <summary>
    /// Picks a status-code-specific fallback message when the response carries no body of its own
    /// (401/403 from ASP.NET Core's own auth short-circuits, 429 from RateLimitFilter, a plain
    /// routing 404) - otherwise every one of these read as the same unhelpful generic error text.
    /// </summary>
    private static string GetDefaultErrorMessageKey(int statusCode) => statusCode switch
    {
        StatusCodes.Status401Unauthorized => ApiResponseConsts.UnauthorizedMessageKey,
        StatusCodes.Status403Forbidden => ApiResponseConsts.ForbiddenMessageKey,
        StatusCodes.Status404NotFound => ApiResponseConsts.NotFoundMessageKey,
        StatusCodes.Status429TooManyRequests => ApiResponseConsts.RateLimitExceededMessageKey,
        _ => ApiResponseConsts.DefaultErrorMessageKey,
    };
}
