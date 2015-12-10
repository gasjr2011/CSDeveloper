using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;
using System.Web.Security;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CSDeveloper.WebPackage
{
    public class AuthenticateAccess : IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get
            {
                return true;
            }
        }

        private static GenericPrincipal GeneratePrincipal(string user, string[] roles = null, int timeout = 30)
        {
            FormsAuthenticationTicket fat = new FormsAuthenticationTicket(user, false, timeout);
            FormsIdentity fid = new FormsIdentity(fat);
            roles = roles == null ? new string[0] : roles;

            return new GenericPrincipal(fid, roles);
        }

        private static void CreateErrorResutl()
        {
            var ret = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden) { ReasonPhrase = "Login failed!" };
            throw new HttpResponseException(ret);
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {

            // 1. Look for credentials in the request.
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            // 2. If there are no credentials, do nothing.
            if (authorization == null || authorization.Scheme != "Basic" )
            {
                return;
            }

            // 3. If there are credentials but the filter does not recognize the 
            //    authentication scheme, do nothing.
            if (authorization.Scheme != "Basic")
            {
                return;
            }
            /*
            // 4. If there are credentials that the filter understands, try to validate them.
            // 5. If the credentials are bad, set the error result.
            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                //context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = null; //ExtractUserNameAndPassword(authorization.Parameter);
            if (userNameAndPasword == null)
            {
                //context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }

            // 6. If the credentials are valid, set principal.
            else
            {
                context.Principal = principal;
            }
            */
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Basic");
            //context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}
