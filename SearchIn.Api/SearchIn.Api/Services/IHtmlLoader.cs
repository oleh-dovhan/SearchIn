using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SearchIn.Api.Services
{
	public interface IHtmlLoader
	{
		string Url { get; set; }

		Task Load();
		Task Load(string url);

		event Action<string, Stream> HtmlDocumentLoaded;
		event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;
	}
}
