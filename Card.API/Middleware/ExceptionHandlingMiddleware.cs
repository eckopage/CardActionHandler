using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;

namespace Card.API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions in the application pipeline.
    /// </summary>
    /// <remarks>
    /// This middleware intercepts exceptions thrown during the request processing pipeline
    /// and provides unified handling for different exception types, ensuring consistent
    /// error responses to the client.
    /// </remarks>
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IStringLocalizer<ExceptionHandlingMiddleware> localizer)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning("Validation error: {Errors}", ex.Errors);
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions of type <see cref="FluentValidation.ValidationException"/> by generating a
        /// structured error response with validation error details.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current HTTP request.
        /// </param>
        /// <param name="ex">
        /// The <see cref="FluentValidation.ValidationException"/> that contains the validation errors.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation to write
        /// the validation error details to the HTTP response.
        /// </returns>
        private Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Title = localizer["ValidationFailed"],
                Status = (int)HttpStatusCode.BadRequest
            };

            return WriteResponseAsync(context, HttpStatusCode.BadRequest, problemDetails);
        }

        /// <summary>
        /// Handles generic exceptions by logging the error details and generating a unified error response.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current HTTP request.
        /// </param>
        /// <param name="ex">
        /// The <see cref="System.Exception"/> that occurred during the request execution.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation to write
        /// the error response to the HTTP response.
        /// </returns>
        private Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            var response = new
            {
                message = localizer["InternalServerError"],
                details = ex.Message
            };

            return WriteResponseAsync(context, HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Writes a structured JSON response to the HTTP context with the specified status code and content.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current HTTP request.
        /// </param>
        /// <param name="statusCode">
        /// The <see cref="System.Net.HttpStatusCode"/> representing the status code of the response.
        /// </param>
        /// <param name="response">
        /// The object containing the response data to be serialized into JSON.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation to write
        /// the JSON response to the HTTP context.
        /// </returns>
        private Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object response)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
