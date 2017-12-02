using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchIn.Api.Services;
using SearchIn.Api.Models;
using SearchIn.Api.Exceptions;

namespace SearchIn.Api.Hubs
{
	public class SearchHub: Hub<IClient>
	{
		private ISearchService searchService;

		public SearchHub(ISearchService searchService)
		{
			this.searchService = searchService;
			this.searchService.UrlStateChanged += UrlStateChangedHandler;
			this.searchService.NewUrlListFound += NewUrlListFoundHandler;
		}

		public override Task OnConnected()
		{
			Clients.Caller.onConnected();
			return base.OnConnected();
		}

		public async Task StartSearch(string startUrl, string searchString, int countUrls, int countThreads)
		{
			try
			{
				if (searchService.SearchState == SearchState.Paused)
					searchService.ResumeSearch();
				else
					await Task.Run(() =>
					{
						try
						{
							searchService.StartSearch(startUrl, searchString, countUrls, countThreads);
						}
						catch (SearchProcessException ex)
						{
							SendErrorMessageToClient(ex.Message);
						}
					});
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}
		public void PauseSearch()
		{
			try
			{
				searchService.PauseSearch();
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}
		public void StopSearch()
		{
			try
			{
				searchService.StopSearch();
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}

		private void UrlStateChangedHandler(UrlStateDto urlStateDto)
		{
			lock (this)
			{
				Clients.Caller.onUrlStateChanged(urlStateDto);
			}
		}
		private void NewUrlListFoundHandler(IEnumerable<UrlDto> urlList)
		{
			lock (this)
			{
				Clients.Caller.onNewUrlListFound(urlList);
			}
		}

		private void SendErrorMessageToClient(string errorMessage)
		{
			lock (this)
			{
				Clients.Caller.onErrorFound(errorMessage);
			}
		}
	}
}