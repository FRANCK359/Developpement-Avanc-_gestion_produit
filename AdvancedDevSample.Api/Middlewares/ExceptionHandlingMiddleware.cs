using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdvancedDevSample.Api.Middlewares
{
    /// <summary>
    /// Middleware pour la gestion centralisée des exceptions
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Une exception non gérée s'est produite: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            object errorResponse;

            switch (exception)
            {
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new
                    {
                        Success = false,
                        Message = exception.Message,
                        ExceptionType = exception.GetType().Name,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        Success = false,
                        Message = exception.Message,
                        ExceptionType = exception.GetType().Name,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case DomainException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        Success = false,
                        Message = exception.Message,
                        ExceptionType = exception.GetType().Name,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new
                    {
                        Success = false,
                        Message = "Une erreur s'est produite lors du traitement de votre demande.",
                        ExceptionType = exception.GetType().Name,
                        Timestamp = DateTime.UtcNow
                    };
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }
}