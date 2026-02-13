using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Api.Middlewares
{
    /// <summary>
    /// Middleware pour le monitoring des performances
    /// </summary>
    public class PerformanceMiddleware
    {
        private const long DefaultWarningThresholdMs = 1000;

        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private readonly long _warningThresholdMs;

        public PerformanceMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMiddleware> logger,
            long warningThresholdMs = DefaultWarningThresholdMs)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _warningThresholdMs = warningThresholdMs;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                LogRequestPerformance(context, stopwatch.ElapsedMilliseconds);
            }
        }

        private void LogRequestPerformance(HttpContext context, long elapsedMilliseconds)
        {
            if (IsSlowRequest(elapsedMilliseconds))
            {
                LogSlowRequest(context, elapsedMilliseconds);
            }
            else
            {
                LogNormalRequest(context, elapsedMilliseconds);
            }
        }

        private bool IsSlowRequest(long elapsedMilliseconds)
        {
            return elapsedMilliseconds > _warningThresholdMs;
        }

        private void LogSlowRequest(HttpContext context, long elapsedMilliseconds)
        {
            _logger.LogWarning(
                "⚠️ Requête lente détectée: {Method} {Path} - Durée: {Duration}ms (seuil: {Threshold}ms)",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds,
                _warningThresholdMs);
        }

        private void LogNormalRequest(HttpContext context, long elapsedMilliseconds)
        {
            _logger.LogDebug(
                "Requête traitée: {Method} {Path} - Durée: {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }
    }
}