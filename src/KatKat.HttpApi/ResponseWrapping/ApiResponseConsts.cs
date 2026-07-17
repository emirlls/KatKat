namespace KatKat.ResponseWrapping;

internal static class ApiResponseConsts
{
    public const string DefaultSuccessMessageKey = "ApiResponse:DefaultSuccessMessage";

    public const string DefaultErrorMessageKey = "ApiResponse:DefaultErrorMessage";

    /// <summary>Fallback message for a bare 401 with no body (e.g. missing/expired bearer token).</summary>
    public const string UnauthorizedMessageKey = "ApiResponse:Unauthorized";

    /// <summary>Fallback message for a bare 403 with no body (e.g. ASP.NET Core's own authorization short-circuit).</summary>
    public const string ForbiddenMessageKey = "ApiResponse:Forbidden";

    /// <summary>Fallback message for a bare 404 with no body (route/entity not found without a more specific message).</summary>
    public const string NotFoundMessageKey = "ApiResponse:NotFound";

    /// <summary>Fallback message for the rate limiter's bare 429 (see RateLimitFilter - it carries no body of its own).</summary>
    public const string RateLimitExceededMessageKey = "ApiResponse:RateLimitExceeded";
}
