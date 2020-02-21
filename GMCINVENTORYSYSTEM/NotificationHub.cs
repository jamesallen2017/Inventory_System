using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GMCINVENTORYSYSTEM
{
	public class NotificationHub : Hub
	{
		[HubMethodName("sendMessages")]
		public static void SendMessages()
		{
			IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
			context.Clients.All.updateMessages();
		}
	}
}