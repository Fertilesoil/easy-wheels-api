using System.Net;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace EasyWheelsApi.Middlewares;

public static class ExceptionMiddlewareExtensions 
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError => 
        {
            appError.Run(async context => 
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if(contextFeature != null)
                {
                    Console.WriteLine($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error"
                    }.ToString());
                }
            });
        });
    }
}
