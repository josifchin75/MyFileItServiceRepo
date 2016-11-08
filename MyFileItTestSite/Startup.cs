using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyFileItTestSite.Startup))]
namespace MyFileItTestSite
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
