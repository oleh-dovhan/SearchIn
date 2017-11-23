using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace SearchIn.Api.Hubs
{
	public class SearchHub: Hub<IClient>
	{
		public override Task OnConnected()
		{
			Clients.Caller.onConnected();
			return base.OnConnected();
		}
	}
}