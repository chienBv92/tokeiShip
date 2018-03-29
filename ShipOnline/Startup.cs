using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShipOnline.Startup))]
namespace ShipOnline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
