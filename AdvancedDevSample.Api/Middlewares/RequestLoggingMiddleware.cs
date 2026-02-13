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
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            LogRequestStart(context);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
                stopwatch.Stop();

                LogRequestSuccess(context, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                LogRequestError(context, stopwatch.ElapsedMilliseconds, ex);
                throw;
            }
        }

        private void LogRequestStart(HttpContext context)
        {
            _logger.LogInformation(
                "→ Début de la requête: {Method} {Path}{QueryString}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);
        }

        private void LogRequestSuccess(HttpContext context, long elapsedMilliseconds)
        {
            var logLevel = DetermineLogLevel(context.Response.StatusCode);

            _logger.Log(
                logLevel,
                "✓ Requête terminée: {Method} {Path} - Status: {StatusCode} - Durée: {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsedMilliseconds);
        }

        private void LogRequestError(HttpContext context, long elapsedMilliseconds, Exception exception)
        {
            _logger.LogError(
                exception,
                "✗ Erreur lors de la requête: {Method} {Path} - Status: {StatusCode} - Durée: {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsedMilliseconds);
        }

        private static LogLevel DetermineLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                _ => LogLevel.Information
            };
        }
    }
}