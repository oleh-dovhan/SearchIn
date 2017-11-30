namespace SearchIn.Api.Services
{
	public interface IHtmlLoaderFactory
	{
		IHtmlLoader Create(string url);
	}
}
