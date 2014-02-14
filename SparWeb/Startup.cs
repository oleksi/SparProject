using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SparWeb.Startup))]
namespace SparWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
