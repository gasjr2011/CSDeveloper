using CSDeveloper.WebPackage;
using System.Web.Http.Filters;

namespace CSDeveloper.WebPackage
{
    public class ResponseModifier : ActionFilterAttribute
    {
        string Token_Key = "HttpLoadKey";

        public ResponseModifier(string keyName = "") : base()
        {
            if (keyName.Trim() != string.Empty)
                this.Token_Key = keyName.Trim();
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var anonControlerFilter = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<NoResponseModifier>();
            var anonActionFilter = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<NoResponseModifier>();

            base.OnActionExecuted(actionExecutedContext);
             
            //--- check of allowanonymouse override
            if (anonActionFilter.Count == 0 && anonControlerFilter.Count == 0) 
            {

                string token = Encryption.CreateToken();

                if (actionExecutedContext.Response.Headers.Contains(this.Token_Key))
                    actionExecutedContext.Response.Headers.Remove(this.Token_Key);

                actionExecutedContext.Response.Headers.Add(this.Token_Key, token);

            }
        }
    }
}