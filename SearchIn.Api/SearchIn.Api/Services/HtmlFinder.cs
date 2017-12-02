using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SearchIn.Api.Services
{
	public class HtmlFinder: IHtmlFinder
	{
		private readonly Regex urlRegex;

		public HtmlFinder()
		{
			urlRegex = new Regex(WebConfigurationManager.AppSettings["urlRegex"], RegexOptions.IgnoreCase);
		}

		public bool Contains(HtmlDocument htmlDoc, string searchString)
		{
			return htmlDoc.DocumentNode
						  .SelectSingleNode(string.Format("//*[text()[contains(., '{0}')]]", searchString)) != null;
		}
		public IEnumerable<string> FindAllUrls(HtmlDocument htmlDoc)
		{
			var hrefNodes = htmlDoc.DocumentNode
						           .SelectNodes("//a[@href]");
			if (hrefNodes != null)
			{
				return hrefNodes.Select(node => node.Attributes["href"].Value)
								.Distinct()
								.Where(url => urlRegex.IsMatch(url));
			}
			else return new List<string>();
		}
	}
}