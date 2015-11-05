using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC4GMAPS.Startup))]
namespace MVC4GMAPS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
