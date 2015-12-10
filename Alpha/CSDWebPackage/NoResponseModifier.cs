using CSDeveloper.WebPackage;
using System.Web.Http.Filters;

namespace CSDeveloper.WebPackage
{
    public class NoResponseModifier : ActionFilterAttribute
    {
        public NoResponseModifier() : base()
        {
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}