using System.Collections.Generic;
using HtmlAgilityPack;

namespace SearchIn.Api.Services
{
	public interface IHtmlFinder
	{
		bool Contains(HtmlDocument htmlDoc, string searchString);
		IEnumerable<string> FindAllUrls(HtmlDocument htmlDoc);
	}
}
