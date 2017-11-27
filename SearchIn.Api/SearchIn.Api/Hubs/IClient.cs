using System.Collections.Generic;
using SearchIn.Api.Models;

namespace SearchIn.Api.Hubs
{
	public interface IClient
	{
		void onConnected();
		void onNewUrlListFound(IEnumerable<UrlDto> urlList);
	}
}
