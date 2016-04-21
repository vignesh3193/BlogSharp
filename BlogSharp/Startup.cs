using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogSharp.Startup))]
namespace BlogSharp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
