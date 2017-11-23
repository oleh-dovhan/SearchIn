namespace SearchIn.Api.Hubs
{
	public interface IClient
	{
		void onConnected();
		void onDisconnected();
	}
}
