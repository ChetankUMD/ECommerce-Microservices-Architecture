using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract the specific header from the request.
            var singedHeader = context.Request.Headers["Api-Gateway"];

            // NULL means request is not coming from the Api-gateway // 503 service unavailable
            if(singedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service not available");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
