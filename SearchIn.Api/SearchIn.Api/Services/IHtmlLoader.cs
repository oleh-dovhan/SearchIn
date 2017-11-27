using System;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SearchIn.Api.Services
{
	public interface IHtmlLoader
	{
		Task Load();

		event Action<string, HtmlDocument> HtmlDocumentLoaded;
		event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;
	}
}
