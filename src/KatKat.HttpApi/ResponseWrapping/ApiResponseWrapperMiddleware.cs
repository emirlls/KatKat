using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KatKat.Localization;
using Microsoft.AspNetCore.Http;
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

        var envelopeJson = BuildEnvelopeJson(context, buffer, localizer);

        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(envelopeJson);
        await context.Response.WriteAsync(envelopeJson);
    }

    private static string BuildEnvelopeJson(HttpContext context, MemoryStream buffer, IStringLocalizer<KatKatResource> localizer)
    {
        var statusCode = context.Response.StatusCode;
        var isSuccess = statusCode is >= 200 and < 300;

        var message = isSuccess
            ? localizer[ApiResponseConsts.DefaultSuccessMessageKey].Value
            : localizer[ApiResponseConsts.DefaultErrorMessageKey].Value;
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
                if (error.TryGetProperty("message", out var messageElement) &&
                    messageElement.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(messageElement.GetString()))
                {
                    message = messageElement.GetString()!;
                }

                if (error.TryGetProperty("validationErrors", out var validationErrors) &&
                    validationErrors.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined))
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
}
