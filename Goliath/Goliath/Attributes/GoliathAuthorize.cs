using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Linq;

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
            System.Collections.Generic.IEnumerable<IgnoreGoliathAuthorizeAttribute> attributes = context.ActionDescriptor.EndpointMetadata.OfType<IgnoreGoliathAuthorizeAttribute>();
            if (attributes.ToList().Count != 0)
            {
                return;
            }
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