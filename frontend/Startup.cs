using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(frontend.Startup))]
namespace frontend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
