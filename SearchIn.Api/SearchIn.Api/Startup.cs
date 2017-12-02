using System;
using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using SearchIn.Api.Hubs;
using SearchIn.Api.Services;

[assembly: OwinStartup(typeof(SearchIn.Api.Startup))]

namespace SearchIn.Api
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			GlobalHost.DependencyResolver.Register(
				typeof(SearchHub),
				() => new SearchHub(new SearchService(new HtmlLoaderFactory(), new HtmlFinder())));
			GlobalHost.Configuration.DisconnectTimeout = new TimeSpan(1, 0, 0);
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR("/search", new HubConfiguration());
			
		}
	}
}
