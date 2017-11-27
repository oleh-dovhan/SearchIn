namespace SearchIn.Api.Services
{
	public class HtmlLoaderFactory : IHtmlLoaderFactory
	{
		public IHtmlLoader Create(string url)
		{
			return new HtmlLoader(url);
		}
	}
}