using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace trackr_api.Filters
{
    public class AuthFilter : IActionFilter
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        public AuthFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Log the incoming request (before the action executes)
            //authenticate before
            _logger.LogInformation("Starting action from AuthFilter: {Action} at {Time}", context.ActionDescriptor.DisplayName, DateTime.UtcNow);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Log the outcome after the action executes
            if (context.Exception != null)
            {
                _logger.LogError("Action {Action} failed with exception from AuthFilter: {Exception} at {Time}",
                    context.ActionDescriptor.DisplayName, context.Exception.Message, DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Completed action from AuthFilter: {Action} at {Time}", context.ActionDescriptor.DisplayName, DateTime.UtcNow);
            }
        }
    }
}
