using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdvancedDevSample.Api.Filters
{
    /// <summary>
    /// Filtre pour la validation automatique des modèles
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = "Erreurs de validation",
                    Errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        ),
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Ne rien faire après l'exécution
        }
    }
}