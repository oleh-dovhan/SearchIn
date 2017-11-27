using System.Collections.Generic;
using HtmlAgilityPack;

namespace SearchIn.Api.Services
{
	public interface IHtmlFinder
	{
		IEnumerable<string> FindAllUrls(HtmlDocument htmlDoc);
		bool Contains(HtmlDocument htmlDoc, string searchString);

	}
}
