using System;

namespace SearchIn.Api.Exceptions
{
	public class SearchProcessException: Exception
	{
		public readonly SearchProcessError Error;

		public SearchProcessException(SearchProcessError error, string message = null) : base(message)
        {
			Error = error;
		}
	}
}