using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchIn.Api.Services
{
	public class HtmlLoader: IHtmlLoader
	{
		private static readonly HttpClient httpClient;
		private string url;

		public event Action<string, Stream> HtmlDocumentLoaded;
		public event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;

		static HtmlLoader()
		{
			httpClient = new HttpClient();
		}
		public HtmlLoader(string url)
		{
			if (url != null) this.url = url;
			else this.url = "";
		}

		public string Url
		{
			get { return url; }
			set { if (url != null) url = value; }
		}

		public async Task Load()
		{
			try
			{
				using (HttpResponseMessage response = await httpClient.GetAsync(url))
				using (HttpContent content = response.Content)
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						using (Stream htmlDoc = await content.ReadAsStreamAsync())
						{
							OnHtmlDocumentLoaded(url, htmlDoc);
						}
					}
					else OnHtmlDocumentLoadFailed(url, response.StatusCode);
				}
			}
			catch (HttpRequestException)
			{
				OnHtmlDocumentLoadFailed(url, HttpStatusCode.RequestTimeout);
			}
		}

		private void OnHtmlDocumentLoaded(string url, Stream stream)
		{
			HtmlDocumentLoaded?.Invoke(url, stream);
		}
		private void OnHtmlDocumentLoadFailed(string url,  HttpStatusCode statusCode)
		{
			HtmlDocumentLoadFailed?.Invoke(url, statusCode);
		}
	}
}