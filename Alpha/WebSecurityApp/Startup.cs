using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSecurityApp.Startup))]
namespace WebSecurityApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
