using System;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject;
using SearchIn.Api.Hubs;
using SearchIn.Api.Services;

[assembly: OwinStartup(typeof(SearchIn.Api.Startup))]

namespace SearchIn.Api
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			GlobalHost.Configuration.DisconnectTimeout = new TimeSpan(1, 0, 0);
			app.UseCors(CorsOptions.AllowAll);

			var kernel = new StandardKernel();
			var resolver = new NinjectSignalRDependencyResolver(kernel);
			kernel.Bind<ISearchService>()
	              .To<SearchService>() 
	              .InSingletonScope()
				  .WithConstructorArgument<IHtmlLoaderFactory>(new HtmlLoaderFactory())
				  .WithConstructorArgument<IHtmlFinder>(new HtmlFinder());
			kernel.Bind(typeof(IHubConnectionContext<dynamic>))
				  .ToMethod(context =>
			          resolver.Resolve<IConnectionManager>()
					          .GetHubContext<SearchHub>()
							  .Clients)
				  .WhenInjectedInto<ISearchService>();
			var config = new HubConfiguration();
			config.Resolver = resolver;
			app.MapSignalR("/search", config);
		}
	}
}
