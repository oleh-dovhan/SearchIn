using System;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SearchIn.Api.Services
{
	public class HtmlLoader: IHtmlLoader
	{
		private HtmlWeb htmlWeb;
		private string url;

		public event Action<string, HtmlDocument> HtmlDocumentLoaded;
		public event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;

		public HtmlLoader(string url)
		{
			htmlWeb = new HtmlWeb()
			{
				PreRequest = request =>
				{
					request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
					return true;
				}
				
			};
			this.url = url;
		}

		public async Task Load()
		{
			var htmlDoc = await Task.Run(() => htmlWeb.Load(url));
			if (htmlWeb.StatusCode == HttpStatusCode.OK)
				OnHtmlDocumentLoaded(url, htmlDoc);
			else OnHtmlDocumentLoadFailed(url, htmlWeb);
		}

		private void OnHtmlDocumentLoaded(string url, HtmlDocument htmlDoc)
		{
			HtmlDocumentLoaded?.Invoke(url, htmlDoc);
		}
		private void OnHtmlDocumentLoadFailed(string url, HtmlWeb htmlWeb)
		{
			HtmlDocumentLoadFailed?.Invoke(url, htmlWeb.StatusCode);
		}
	}
}