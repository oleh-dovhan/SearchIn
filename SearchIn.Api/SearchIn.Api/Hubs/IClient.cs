using System.Collections.Generic;
using SearchIn.Api.Models;

namespace SearchIn.Api.Hubs
{
	public interface IClient
	{
		void onConnected();
		void onUrlStateChanged(UrlStateDto urlStateDto);
		void onNewUrlListFound(IEnumerable<UrlDto> urlList);
		void onErrorFound(string errorMessage);
	}
}
