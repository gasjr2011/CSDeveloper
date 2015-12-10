using System.Web.Mvc;
using CSDeveloper.WebPackage;
namespace WebSecurityApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            
            filters.Add(new HandleErrorAttribute());
        }
    }
}
