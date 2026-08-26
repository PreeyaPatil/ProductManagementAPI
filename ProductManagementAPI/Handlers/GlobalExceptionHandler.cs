using Microsoft.AspNetCore.Diagnostics;
using ProductManagementAPI.Common;
using ProductManagementAPI.Exceptions;

namespace ProductManagementAPI.Handlers
{
    // Provides centralized exception handling for the entire application.
    // It converts unhandled exceptions into consistent JSON error responses.
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        // Stores the logger used to record exception details.
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // ILogger is provided through ASP.NET Core dependency injection.
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            // Store the injected logger for later use.
            _logger = logger;
        }

        // This method is automatically called when an unhandled
        // exception reaches the Global Exception Handler.
        public async ValueTask<bool> TryHandleAsync(
            // Contains information about the current HTTP request and response.
            HttpContext httpContext,

            // Represents the exception that occurred.
            Exception exception,

            // Allows the response-writing operation to be cancelled
            // when the HTTP request is cancelled.
            CancellationToken cancellationToken)
        {
            // Select the appropriate HTTP status code
            // based on the type of exception.
            var statusCode = exception switch
            {
                // An invalid request produces HTTP 400 Bad Request.
                BadRequestException =>
                    StatusCodes.Status400BadRequest,

                // A missing resource produces HTTP 404 Not Found.
                NotFoundException =>
                    StatusCodes.Status404NotFound,

                // Any other unexpected exception produces HTTP 500 Internal Server Error.
                _ =>
                    StatusCodes.Status500InternalServerError
            };

            // Select the error message that should be returned to the client.
            var message = exception switch
            {
                // For known exceptions, return the meaningful
                // message provided when the exception was thrown.
                BadRequestException =>
                    exception.Message,

                NotFoundException =>
                    exception.Message,

                // Do not expose internal technical details for unexpected exceptions.
                _ =>
                    "An unexpected error occurred while processing the request."
            };

            // Checks whether the exception is an unexpected server error.
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                // Log the complete exception as an error.
                // The actual exception details are stored in the logs
                // but are not sent to the client.

                // TraceIdentifier uniquely identifies
                // the current HTTP request.
                _logger.LogError(
                    exception,
                    "An unhandled exception occurred. " +
                    "Trace ID: {TraceId}",
                    httpContext.TraceIdentifier);
            }
            else
            {
                // Log known application errors as warnings
                // because they are expected failures rather
                // than unexpected system errors.
                _logger.LogWarning(
                    "Request failed with status code " +
                    "{StatusCode}. Message: {Message}. " +
                    "Trace ID: {TraceId}",
                    statusCode,
                    message,
                    httpContext.TraceIdentifier);
            }

            // Create a consistent failure response
            // using the common ApiResponse<T> format.
            var response = ApiResponse<object>.CreateFailure(
                    // HTTP status code selected above.
                    statusCode,

                    // Safe message to return to the client.
                    message,

                    // Include the request trace ID so that the error
                    // can be matched with application logs.
                    traceId: httpContext.TraceIdentifier);

            // Sets the HTTP status code of the response.
            httpContext.Response.StatusCode = statusCode;

            // Inform the client that the response body contains JSON data.
            httpContext.Response.ContentType = "application/json";

            // Convert the ApiResponse object into JSON
            // and write it to the HTTP response body.
            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            // Return true to indicate that the exception
            // has been successfully handled.
            return true;
        }
    }
}
