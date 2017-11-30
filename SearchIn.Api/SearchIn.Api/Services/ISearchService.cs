using System;
using System.Collections.Generic;
using SearchIn.Api.Models;

namespace SearchIn.Api.Services
{
	public interface ISearchService
	{
		event Action<UrlStateDto> UrlStateChanged;
		event Action<IEnumerable<UrlDto>> NewUrlListFound;

		SearchState SearchState { get; }

		void StartSearch(string startUrl, string searchString, int countUrls, int countThreads);
		void PauseSearch();
		void ResumeSearch();
		void StopSearch();
	}
}
