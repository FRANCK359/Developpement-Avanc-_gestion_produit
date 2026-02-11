using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Api.Filters
{
    /// <summary>
    /// Filtre pour le logging des actions des contrôleurs
    /// </summary>
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(
                "Exécution de l'action: {Controller}.{Action}",
                context.Controller.GetType().Name,
                context.ActionDescriptor.DisplayName);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                _logger.LogError(
                    context.Exception,
                    "Erreur dans l'action: {Controller}.{Action}",
                    context.Controller.GetType().Name,
                    context.ActionDescriptor.DisplayName);
            }
            else
            {
                _logger.LogInformation(
                    "Action terminée: {Controller}.{Action}",
                    context.Controller.GetType().Name,
                    context.ActionDescriptor.DisplayName);
            }
        }
    }
}