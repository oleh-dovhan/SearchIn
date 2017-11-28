using System;
using System.IO;
using System.Net;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SearchIn.Api.Models;
using SearchIn.Api.Exceptions;
using SearchIn.Api.Infrastructure;

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

		private string searchString; 
		private int countUrls;

		private volatile SearchState searchState;

		public event Action<IEnumerable<UrlDto>> NewUrlListFound;
		public event Action<UrlStateDto> UrlStateChanged;

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
		private void HtmlDocumentLoadedHandler(string url, Stream stream)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(stream);

			bool contains = htmlFinder.Contains(htmlDoc, searchString);
			var urlStateDto = new UrlStateDto
			{
				Id = urlList.ToList().IndexOf(url),
				ScanState = contains ? ScanState.Found: ScanState.NotFound
			};
			OnUrlStateChanged(urlStateDto);

		    var childUrlList = htmlFinder.FindAllUrls(htmlDoc);
			var childUrlDtoList = new List<UrlDto>();
			foreach (var childUrl in childUrlList)
			{
				if (urlList.Count >= countUrls)
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
			var urlStateDto = new UrlStateDto
			{
				Id = urlList.ToList().IndexOf(url),
				ScanState = ScanState.Error
			};
			OnUrlStateChanged(urlStateDto);
		}

		public void StartSearch(string startUrl, string searchString, int countUrls, int countThreads)
		{
			if (searchState == SearchState.Stopped)
			{
				if (!IsStartSearchInputDataValid(startUrl, searchString, countUrls, countThreads))
				{
					throw new SearchProcessException(SearchProcessError.IncorrectInputData, "Incorrect input data.");
				}

				this.searchString = searchString;
				this.countUrls = countUrls;

				searchState = SearchState.Running;

				var lcts = new LimitedConcurrencyLevelTaskScheduler(countThreads);
				var taskFactory = new TaskFactory(lcts);

				urlQueue.Enqueue(startUrl);
				string currUrl; 
				while (searchState != SearchState.Stopped || !urlQueue.IsEmpty)
				{
					if (searchState == SearchState.Running && urlQueue.TryDequeue(out currUrl))
					{
						var htmlLoader = htmlLoaderFactory.Create(currUrl);
						htmlLoader.HtmlDocumentLoaded += HtmlDocumentLoadedHandler;
						htmlLoader.HtmlDocumentLoadFailed += HtmlDocumentLoadFailedHandler;
						taskFactory.StartNew(async () => await htmlLoader.Load());
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
		private void OnUrlStateChanged(UrlStateDto urlStateDto)
		{
			UrlStateChanged?.Invoke(urlStateDto);
		}
	}
}