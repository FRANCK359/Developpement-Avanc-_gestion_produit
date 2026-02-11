using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Api.Filters
{
    /// <summary>
    /// Filtre global pour la gestion des exceptions
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Exception non gérée");

            var response = new
            {
                Success = false,
                Message = "Une erreur s'est produite lors du traitement de votre demande.",
                Detailed = _environment.IsDevelopment() ? context.Exception.Message : null,
                ExceptionType = context.Exception.GetType().Name,
                Timestamp = DateTime.UtcNow
            };

            context.Result = context.Exception switch
            {
                NotFoundException => new NotFoundObjectResult(response),
                ValidationException => new BadRequestObjectResult(response),
                DomainException => new BadRequestObjectResult(response),
                _ => new ObjectResult(response) { StatusCode = 500 }
            };

            context.ExceptionHandled = true;
        }
    }
}