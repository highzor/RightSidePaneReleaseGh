using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(ServiceCRM.Startup))]
namespace ServiceCRM
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			app.Map("/signalr", map =>
			{
				map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                };
                map.RunSignalR(hubConfiguration);
            });
		}
    }
}
