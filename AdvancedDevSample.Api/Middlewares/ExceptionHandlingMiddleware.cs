using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Exceptions;
using System.Net;
using System.Text.Json;


namespace AdvancedDevSample.Api.Middlewares
{
    /// <summary>
    /// Middleware pour la gestion centralisée des exceptions
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            LogException(exception);

            var statusCode = DetermineStatusCode(exception);
            var errorResponse = CreateErrorResponse(exception, statusCode);

            await WriteJsonResponseAsync(context, statusCode, errorResponse);
        }

        private void LogException(Exception exception)
        {
            _logger.LogError(exception,
                "Exception non gérée: {ExceptionType} - {Message}",
                exception.GetType().Name,
                exception.Message);
        }

        private static HttpStatusCode DetermineStatusCode(Exception exception)
        {
            return exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                ValidationException => HttpStatusCode.BadRequest,
                DomainException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static object CreateErrorResponse(Exception exception, HttpStatusCode statusCode)
        {
            var message = statusCode == HttpStatusCode.InternalServerError
                ? "Une erreur s'est produite lors du traitement de votre demande."
                : exception.Message;

            return new
            {
                Success = false,
                Message = message,
                ExceptionType = exception.GetType().Name,
                Timestamp = DateTime.UtcNow
            };
        }

        private static async Task WriteJsonResponseAsync(HttpContext context, HttpStatusCode statusCode, object errorResponse)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}