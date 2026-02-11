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
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private readonly long _warningThreshold; // en millisecondes

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _warningThreshold = 1000; // 1 seconde
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > _warningThreshold)
            {
                _logger.LogWarning(
                    "Requête lente détectée: {Method} {Path} - Durée: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}