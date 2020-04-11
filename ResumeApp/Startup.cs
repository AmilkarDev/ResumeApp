using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ResumeApp.Startup))]
namespace ResumeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
