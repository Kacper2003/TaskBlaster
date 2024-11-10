using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using TaskBlaster.TaskManagement.Models;
using System.Text.Json;
using TaskBlaster.TaskManagement.Models.Exceptions;

namespace TaskBlaster.TaskManagement.API.ExceptionHandlerExtensions;

public static class ExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        // TODO: Implement
        app.UseExceptionHandler(error => 
        {
            error.Run(async context => {
                // * Retrieve the exception handler feature
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature != null)
                {
                    var exception = exceptionHandlerFeature.Error;
                    var statusCode = (int) HttpStatusCode.InternalServerError; // * 500
                    context.Response.ContentType = "application/json";

                    if (exception is ResourceNotFoundException)
                    {
                        statusCode = (int) HttpStatusCode.NotFound; // * 404
                    }
                    else if (exception is ModelFormatException)
                    {
                        statusCode = (int) HttpStatusCode.PreconditionFailed; // * 412
                    }
                    else if (exception is ArgumentOutOfRangeException)
                    {
                        statusCode = (int) HttpStatusCode.BadRequest; // * 400
                    }
                    else if (exception is InvalidOperationException)
                    {
                        statusCode = (int) HttpStatusCode.Conflict; // * 409
                    }

                    context.Response.StatusCode = statusCode;

                    // * Create an instance and store it, as it is used twice
                    var exceptionModel = new ExceptionModel
                    {
                        StatusCode = statusCode,
                        ExceptionMessage = statusCode == 500 ? "Internal Server Error" : exception.Message,
                    };

                    // * Write the exception to the response
                    var json = JsonSerializer.Serialize(exceptionModel);
                    await context.Response.WriteAsync(json);
                }
            });
        });
    }
}    