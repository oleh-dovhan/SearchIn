namespace SearchIn.Api.Models
{
	public class UrlStateDto
	{
		public int Id { get; set; }
		public ScanState ScanState { get; set; }

		public static UrlStateDto FromModel(UrlDto urlDto)
		{
			return new UrlStateDto()
			{
				Id = urlDto.Id,
				ScanState = urlDto.ScanState
			};
		}
	}
}