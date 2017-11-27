using System;
using System.Collections.Generic;
using SearchIn.Api.Models;

namespace SearchIn.Api.Services
{
	public interface ISearchService
	{
		void StartSearch(string startUrl, string searchString, int countUrls, int countThreads);
		void PauseSearch();
		void ResumeSearch();
		void StopSearch();

		event Action<IEnumerable<UrlDto>> NewUrlListFound;
	}
}
