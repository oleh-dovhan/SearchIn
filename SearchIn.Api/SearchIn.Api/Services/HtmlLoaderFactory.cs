namespace SearchIn.Api.Services
{
	public class HtmlLoaderFactory : IHtmlLoaderFactory
	{
		public IHtmlLoader Create()
		{
			return new HtmlLoader();
		}
		public IHtmlLoader Create(string url)
		{
			return new HtmlLoader(url);
		}
	}
}