using System.Web.Mvc;

namespace Goliath.Attributes
{
    public class IgnoreGoliathAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
