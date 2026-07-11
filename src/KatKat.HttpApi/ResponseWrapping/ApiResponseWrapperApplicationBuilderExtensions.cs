using Microsoft.AspNetCore.Builder;

namespace KatKat.ResponseWrapping;

public static class ApiResponseWrapperApplicationBuilderExtensions
{
    public static IApplicationBuilder UseKatKatApiResponseWrapper(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiResponseWrapperMiddleware>();
    }
}
