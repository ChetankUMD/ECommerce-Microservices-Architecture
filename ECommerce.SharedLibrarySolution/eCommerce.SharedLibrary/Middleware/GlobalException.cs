using Azure.Core;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declaring default Variables
            string title = "Error";
            string message = "Sorry, Internal server error occurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                await next(context);

                // Check if the Response are too many // 429 status code.
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Check if the request is Unautorized // 401 Status Code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Check if the request is forbidden // 403 Status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Forbiddeb";
                    message = "The request is not allowed/required to access";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log Original Exceptions | Console | degub | file.
                LogException.LogExceptions(ex);

                // check if Exception is Timeout.
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    statusCode= (int)StatusCodes.Status408RequestTimeout;
                    message = "Request Timeout.. try agian";
                    await ModifyHeader(context, title, message, statusCode);
                }
                else
                {
                    // if non of this above exception are caught || send default message;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Title = title,
                Detail = message,
                Status = statusCode
            }));
        }
    }
}
