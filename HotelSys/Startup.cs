using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HotelSys.Startup))]
namespace HotelSys
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
