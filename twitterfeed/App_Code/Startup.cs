using Microsoft.Owin;
using Owin;
using twitterfeed;

[assembly: OwinStartup(typeof(twitterfeed.Startup))]
namespace twitterfeed
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}