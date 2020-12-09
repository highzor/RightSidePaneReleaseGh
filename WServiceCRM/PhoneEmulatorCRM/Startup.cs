using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(PhoneEmulatorCRM.Startup))]
namespace PhoneEmulatorCRM
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}