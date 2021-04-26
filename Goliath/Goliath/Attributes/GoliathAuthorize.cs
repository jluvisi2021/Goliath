using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Goliath.Attributes
{
    public class GoliathAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _title;

        public GoliathAuthorizeAttribute(string title)
        {
            _title = title;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "UserPanel",
                    action = "NotAuthenticated",
                    title = _title
                }));
            }
        }
    }
}