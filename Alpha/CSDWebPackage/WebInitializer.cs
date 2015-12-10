using System.Web.Mvc;
using System.Web.Http.Filters;
namespace CSDeveloper.WebPackage
{
    public static class WebInitializer
    {
        public static void RegisterGlobalViewFilters(GlobalFilterCollection filters, string homeView = "", string accessErrorMessage = "You have not login.")
        {
            AuthorizePageAccess  auth = new AuthorizePageAccess() { HomeView = homeView, AccessErrorMessage  = accessErrorMessage };
            HandleErrorAttribute errAtt = new HandleErrorAttribute();

            if (!filters.Contains(auth)) filters.Add(auth); 
            if (!filters.Contains(errAtt)) filters.Add(errAtt);
            
        }

        public static void RegisterGlobalApiFilters(HttpFilterCollection filters, string homeView = "", string accessErrorMessage = "You have not login.")
        {

            AuthorizeApiAccess auth = new AuthorizeApiAccess();
            ResponseModifier resp = new ResponseModifier();
            AuthenticateAccess auty = new AuthenticateAccess();
            //HandleErrorAttribute errAtt = new HandleErrorAttribute();

            if (!filters.Contains(auty)) filters.Add(auty);
            if (!filters.Contains(auth)) filters.Add(auth);
            if (!filters.Contains(resp)) filters.Add(resp);
            //if (!filters.Contains(errAtt)) filters.Add(errAtt);
        }

    }
}
