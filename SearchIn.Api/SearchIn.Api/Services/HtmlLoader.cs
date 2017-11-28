using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchIn.Api.Services
{
	public class HtmlLoader: IHtmlLoader
	{
		private string url;

		public event Action<string, Stream> HtmlDocumentLoaded;
		public event Action<string, HttpStatusCode> HtmlDocumentLoadFailed;

		public HtmlLoader()
		{
			url = "";
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
			await Load(url);
		}
		public async Task Load(string url)
		{
			using (var client = new HttpClient())
			using (HttpResponseMessage response = await client.GetAsync(url))
			using (HttpContent content = response.Content)
			{
				if (response.StatusCode == HttpStatusCode.OK)
				{
					Stream htmlDoc = await content.ReadAsStreamAsync();
					OnHtmlDocumentLoaded(url, htmlDoc);
				}
				else OnHtmlDocumentLoadFailed(url, response.StatusCode);
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