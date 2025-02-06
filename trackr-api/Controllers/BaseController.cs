using Microsoft.AspNetCore.Mvc;
using trackr_api.Filters;  // Add the namespace for the filter

namespace trackr_api.Controllers
{
    [ServiceFilter(typeof(ActionLoggingFilter))]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleError(Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }
}
