using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SearchIn.Api.Services
{
	public interface IHtmlLoader
	{
		event Action<string, Stream> HtmlDocumentLoaded;
		event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;

		string Url { get; set; }

		Task Load();
	}
}
