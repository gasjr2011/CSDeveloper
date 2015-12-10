using System.Web.Mvc;
using System.Web;
using System.Security.Principal;
using CSDeveloper;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
namespace CSDeveloper.WebPackage
{
    public delegate bool CheckAuthorization(IIdentity iden, string controller, string action);

    public class AuthorizePageAccess: System.Web.Mvc.AuthorizeAttribute 
    {
        public CheckAuthorization AuthorizationProcedure = (IIdentity a, string b, string c) => {
            return true;
        };

        public string HomeView { get; set; }
        public string AccessErrorMessage { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //--- run base authorization
            base.OnAuthorization(filterContext);

            var anonControlerFilter = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
            var anonActionFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
            var anonAuthorizedFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            //--- check of allowanonymouse override
            if ((anonActionFilter.Length == 0 && anonControlerFilter.Length  == 0 && filterContext.Result == null)  || anonAuthorizedFilter.Length > 0)
            {
                //--- not authenticated
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated || 
                    !this.AuthorizationProcedure(filterContext.HttpContext.User.Identity,
                                                 filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                                 filterContext.ActionDescriptor.ActionName))
                {
                    if (this.HomeView != string.Empty)
                        filterContext.Result = new RedirectResult(this.HomeView, true);
                    else
                        filterContext.Result = new HttpStatusCodeResult(403, this.AccessErrorMessage);
                }
            }
        }
    }
}
