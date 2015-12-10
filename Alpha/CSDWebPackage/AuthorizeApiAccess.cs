using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Security.Principal;

namespace CSDeveloper.WebPackage
{
    public class AuthorizeApiAccess : System.Web.Http.AuthorizeAttribute
    {
        public CheckAuthorization AuthorizationProcedure = (IIdentity a, string b, string c) => { return true; };
        string Token_Key = "HttpLoadKey";
        int Duration = 0;

        public AuthorizeApiAccess(string keyName = "", int duration = 3):base()
        {
            if (keyName.Trim() != string.Empty)
                this.Token_Key = keyName.Trim();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var anonControlerFilter = actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
            var anonActionFilter = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
            var anonAuthorizedFilter = actionContext.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>();

            //--- check of allowanonymouse override
            if ((anonActionFilter.Count  == 0 && anonControlerFilter.Count  == 0) || anonAuthorizedFilter.Count > 0)
            {
                if (!Authorize(actionContext))
                    HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            //--- this triggers login, password basic auth:  challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            throw new HttpResponseException(challengeMessage);
        }

        private bool Authorize(HttpActionContext actionContext)
        {
            bool ret = false;

            //----- validate use token
            if (actionContext.Request.Headers.Contains(this.Token_Key))
                ret = CSDeveloper.Encryption.Validated(actionContext.Request.Headers.Contains(this.Token_Key)
                                                        .ToString().Trim(), this.Duration);

            //---- validate authentication token
            if (!ret)
            {
                IPrincipal principal = actionContext.RequestContext.Principal;
                ret = principal.Identity.IsAuthenticated;

                if (ret)
                    ret = this.AuthorizationProcedure(principal.Identity, 
                                                      actionContext.ControllerContext.ControllerDescriptor.ControllerName,
                                                      actionContext.ActionDescriptor.ActionName); 
            }

            return ret;
        }
    }
}
