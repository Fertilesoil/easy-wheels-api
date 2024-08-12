using System.Net;
using System.Text.Json;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyWheelsApi.GlobalExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) =>
            this._logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(exception, exception.Message);

            int statusCode;
            string message;
            string title;

            if (exception is CustomException customException)
            {
                statusCode = customException.StatusCode;
                message = customException.Message;
                title = customException.Title;
            } else
            // else if (exception is typeof(IdentityResult) identityResult)
            // {

            // } else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                title = "Api error";
            }

            var details = new ProblemDetails()
            {
                Detail = message,
                Instance = httpContext.Request.Path,
                Status = statusCode,
                Title = title
            };

            var response = JsonSerializer.Serialize(details);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            
            await httpContext.Response.WriteAsync(response, cancellationToken);

            return true;
        }
    }
}