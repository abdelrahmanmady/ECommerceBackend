using ECommerce.Business.DTOs.Errors;
using ECommerce.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ECommerce.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";
            string? detail = null;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = "Resource not found.";
                    detail = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Invalid request data.";
                    detail = exception.Message;
                    break;

                case ConflictException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = "Data conflict occurred.";
                    detail = exception.Message;
                    break;

                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "You are not authorized to perform this action.";
                    detail = exception.Message;
                    break;
            }


            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Server Error: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Client Error ({StatusCode}): {Message}", statusCode, exception.Message);
            }

            response.StatusCode = statusCode;

            var errorResponse = new ApiErrorResponseDto
            {
                StatusCode = statusCode,
                Message = message,
                Detail = detail
            };

            await response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
