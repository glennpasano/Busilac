using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Busilac.Startup))]
namespace Busilac
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
