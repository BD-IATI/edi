using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AIMS_BD_IATI.Web.Startup))]
namespace AIMS_BD_IATI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
