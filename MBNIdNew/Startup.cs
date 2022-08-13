using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OMSTeleSale.Startup))]
namespace OMSTeleSale
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
