using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace api_Product.CommonFilter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]  // Ensure this is correct
    public class RouteBaseActionFilterData : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            // Check if the user is authenticated
            if (user.Identity?.IsAuthenticated == true)
            {
                // Get the "allowed_routes" claim
                var allowedRoutesClaim = user.Claims.FirstOrDefault(c => c.Type == "allowed_routes")?.Value;

                if (allowedRoutesClaim != null)
                {                    
                    var allowedRoutes = allowedRoutesClaim.Split(',');                    
                    var currentRoute = context.HttpContext.Request.Path.Value;                    
                    if (!allowedRoutes.Contains(currentRoute))
                    {                       
                        context.Result = new ForbidResult();
                    }
                }
            }

            base.OnActionExecuting(context);  // Ensure base class logic is also executed
        }

        public override void OnActionExecuted(ActionExecutedContext context) { }
    }
}
