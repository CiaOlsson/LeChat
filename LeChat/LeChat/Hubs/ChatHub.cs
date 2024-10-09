using Microsoft.AspNetCore.SignalR;

namespace LeChat.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message)
		{
			// När man tar emot ett meddelande så skickas det ut till Alla användare/klienter. Även den som skickade det. 
			await Clients.All.SendAsync("RecieveMessage", user, message);
		}
	}
}
