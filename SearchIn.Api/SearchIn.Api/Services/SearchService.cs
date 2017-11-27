using System;
using System.Net;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SearchIn.Api.Models;
using SearchIn.Api.Exceptions;
using System.Threading.Tasks;
using System.Threading;

namespace SearchIn.Api.Services
{
	public class SearchService : ISearchService
	{
		private readonly int maxCountUrls;
		private readonly int maxCountThreads;
		private readonly Regex urlRegex;

		private IHtmlLoaderFactory htmlLoaderFactory;
		private IHtmlFinder htmlFinder;

		private BlockingCollection<string> urlList;
		private ConcurrentQueue<string> urlQueue;

		private volatile int c = 0;

		private volatile SearchState searchState;

		public event Action<IEnumerable<UrlDto>> NewUrlListFound;
		public event Action<string, HttpStatusCode> PageLoadFailed;

		public SearchState SearchState
		{
			get { return searchState; }
		}
		public int MaxCountUrls
		{
			get { return maxCountUrls; }
		}
		public int MaxCountThreads
		{
			get { return maxCountThreads; }
		}


		public SearchService(IHtmlLoaderFactory htmlLoaderFactory, IHtmlFinder htmlFinder)
		{
			if (!int.TryParse(WebConfigurationManager.AppSettings["maxCountUrls"], out maxCountUrls))
				maxCountUrls = 1000;
			if (!int.TryParse(WebConfigurationManager.AppSettings["maxCountThreads"], out maxCountThreads))
				maxCountThreads = 4;
			urlRegex = new Regex(WebConfigurationManager.AppSettings["urlRegex"], RegexOptions.IgnoreCase);

			this.htmlLoaderFactory = htmlLoaderFactory;
			this.htmlFinder = htmlFinder;

			urlList = new BlockingCollection<string>();
			urlQueue = new ConcurrentQueue<string>();

			searchState = SearchState.Stopped;
		}

		private bool IsStartSearchInputDataValid(string startUrl, string searchString, int countUrls, int countThreads)
		{
			return countUrls > 0 && countUrls <= maxCountUrls
				                 && countThreads > 0 
								 && countThreads <= maxCountThreads
				                 && !string.IsNullOrWhiteSpace(searchString)
				                 && urlRegex.IsMatch(startUrl);
		}
		private void HtmlDocumentLoadedHandler(string url, HtmlDocument htmlDoc)
		{
		    var childUrlList = htmlFinder.FindAllUrls(htmlDoc);
			var childUrlDtoList = new List<UrlDto>();
			foreach (var childUrl in childUrlList)
			{
				if (urlList.Count >= c)
				{
					searchState = SearchState.Stopped;
					break;
				}
				if (!urlList.Contains(childUrl))
				{
					urlList.Add(childUrl);
					urlQueue.Enqueue(childUrl);
					childUrlDtoList.Add(new UrlDto
					{
						Id = urlList.Count - 1,
						ScanState = ScanState.Downloading,
						Value = childUrl
					});
				}
			}
			if (childUrlDtoList.Count > 0)
			{
				OnNewUrlListFound(childUrlDtoList);
			}
		}
		private void HtmlDocumentLoadFailedHandler(string url, HttpStatusCode statusCode)
		{
			OnPageLoadFailed(url, statusCode);
		}

		public void StartSearch(string startUrl, string searchString, int countUrls, int countThreads)
		{
			if (searchState == SearchState.Stopped)
			{
				if (!IsStartSearchInputDataValid(startUrl, searchString, countUrls, countThreads))
				{
					throw new SearchProcessException(SearchProcessError.IncorrectInputData, "Incorrect input data.");
				}

				searchState = SearchState.Running;
				TaskFactory taskFactory = new TaskFactory();
				c = countUrls;

				urlQueue.Enqueue(startUrl);
				string currUrl; 
				while (urlList.Count < countUrls || !urlQueue.IsEmpty)
				{
					if (urlQueue.TryDequeue(out currUrl))
					{
						var htmlLoader = htmlLoaderFactory.Create(currUrl);
						htmlLoader.HtmlDocumentLoaded += HtmlDocumentLoadedHandler;
						htmlLoader.HtmlDocumentLoadFailed += HtmlDocumentLoadFailedHandler;
						//Thread.Sleep(100);
						taskFactory.StartNew(async () => await htmlLoader.Load());
						//htmlLoader.Load();
					} 
				}
			}
			else
			{
				throw new SearchProcessException(SearchProcessError.ProcessCanNotBeRun, "Can't run new search process when current process is running.");
			}
		}
		public void PauseSearch()
		{
			if (searchState == SearchState.Running)
			{
				searchState = SearchState.Paused;
			}
			else if (searchState == SearchState.Stopped)
			{
				throw new SearchProcessException(SearchProcessError.ProcessNotStarted, "Can't pause not running process.");
			}
		}
		public void ResumeSearch()
		{
			if (searchState == SearchState.Paused)
			{
				searchState = SearchState.Running;
			}
			else if (searchState == SearchState.Stopped)
			{
				throw new SearchProcessException(SearchProcessError.ProcessNotStarted, "Can't resume not running process.");
			}
		}
		public void StopSearch()
		{
			if (searchState == SearchState.Running)
			{
				searchState = SearchState.Stopped;
			}
			else if (searchState == SearchState.Paused)
			{
				searchState = SearchState.Stopped;
			}
		}

		private void OnNewUrlListFound(IEnumerable<UrlDto> urlList)
		{
			NewUrlListFound?.Invoke(urlList);
		}
		private void OnPageLoadFailed(string url, HttpStatusCode statusCode)
		{
			PageLoadFailed?.Invoke(url, statusCode);
		}
	}
}