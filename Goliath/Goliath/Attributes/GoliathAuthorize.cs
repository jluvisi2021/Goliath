using Goliath.Controllers;
using Goliath.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Goliath.Attributes
{
    /// <summary>
    /// Validates if the User is signed in and if not then it redirects to <see
    /// cref="UserPanelController.NotAuthenticated(string)" />.
    /// </summary>
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
                    controller = GoliathControllers.UserPanelController,
                    action = nameof(UserPanelController.NotAuthenticated),
                    title = _title
                }));
            }
        }
    }
}