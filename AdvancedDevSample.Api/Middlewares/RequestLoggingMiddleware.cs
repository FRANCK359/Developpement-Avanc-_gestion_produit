using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Api.Middlewares
{
    /// <summary>
    /// Middleware pour le logging des requêtes HTTP
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            _logger.LogInformation(
                "Début de la requête {Method} {Path}",
                request.Method,
                request.Path);

            try
            {
                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Fin de la requête {Method} {Path} - Status: {StatusCode} - Durée: {Duration}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Erreur lors de la requête {Method} {Path} - Status: {StatusCode} - Durée: {Duration}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}