using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Goliath.Attributes
{
    //Credit: https://stackoverflow.com/a/32807640
    /// <summary>
    /// An attribute to be applied to an HttpPost controller.<br />
    /// Prevents the double submission of a controller by checking the anti-forgery token.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PreventDuplicateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.HasFormContentType && context.HttpContext.Request.Form.ContainsKey("__RequestVerificationToken"))
            {
                string currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
                string lastToken = context.HttpContext.Session.GetString("LastProcessedToken");

                if (lastToken == currentToken)
                {
                    context.ModelState.AddModelError(string.Empty, "You already submitted this form.");
                }
                else
                {
                    context.HttpContext.Session.SetString("LastProcessedToken", currentToken);
                }
            }
        }
    }
}
