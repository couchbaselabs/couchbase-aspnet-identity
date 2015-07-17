using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CouchbaseAspNet.Identity.Example.Startup))]
namespace CouchbaseAspNet.Identity.Example
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
