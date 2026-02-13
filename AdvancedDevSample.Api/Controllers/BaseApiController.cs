
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdvancedDevSample.Application.Exceptions;
using System;
using System.Threading.Tasks;

namespace AdvancedDevSample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public abstract class BaseApiController<TController> : ControllerBase where TController : class
    {
        protected readonly ILogger<TController> Logger;

        protected BaseApiController(ILogger<TController> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<ActionResult<T>> ExecuteAsync<T>(
            Func<Task<T>> action,
            string logMessage,
            params object[] logParameters)
        {
            Logger.LogInformation(logMessage, logParameters);

            try
            {
                var result = await action();
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, "Validation failed");
                return BadRequest(new { message = ex.Message });
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        protected async Task<ActionResult<T>> ExecuteCreationAsync<T>(
            Func<Task<T>> action,
            Func<T, object> routeValues,
            string actionName,
            string logMessage,
            params object[] logParameters) where T : class
        {
            Logger.LogInformation(logMessage, logParameters);

            try
            {
                var result = await action();
                return CreatedAtAction(actionName, routeValues(result), result);
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, "Creation failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        protected async Task<IActionResult> ExecuteDeleteAsync(
            Func<Task> action,
            string logMessage,
            params object[] logParameters)
        {
            Logger.LogInformation(logMessage, logParameters);

            try
            {
                await action();
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}