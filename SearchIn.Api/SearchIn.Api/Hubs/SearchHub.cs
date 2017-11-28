using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchIn.Api.Services;
using SearchIn.Api.Models;

namespace SearchIn.Api.Hubs
{
	public class SearchHub: Hub<IClient>
	{
		private ISearchService searchService;

		public SearchHub(ISearchService searchService)
		{
			this.searchService = searchService;
			this.searchService.NewUrlListFound += NewUrlListFoundHandler;
		}

		public override Task OnConnected()
		{
			Clients.Caller.onConnected();
			return base.OnConnected();
		}
		public async Task StartSearch(string startUrl, string searchString, int countUrls, int countThreads)
		{
			await Task.Run(() => searchService.StartSearch(startUrl, searchString, countUrls, countThreads));
		}
		public void PauseSearch()
		{
			searchService.PauseSearch();
		}
		public void StopSearch()
		{
			searchService.StopSearch();
		}

		private void NewUrlListFoundHandler(IEnumerable<UrlDto> urlList)
		{
			Clients.Caller.onNewUrlListFound(urlList);
		}
		private void PageLoadFailedHandler(IEnumerable<UrlDto> urlList)
		{
			Clients.Caller.onNewUrlListFound(urlList);
		}
	}
}