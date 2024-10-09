using LeChat.Data;
using LeChat.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace LeChat.Hubs
{
	public class ChatHub : Hub
	{
		private readonly ChatMessageService _messageService;

		public ChatHub(ChatMessageService messageService)
		{
			_messageService = messageService;
		}
		public async Task SendMessage(string user, string message)
		{
			
			await _messageService.SaveMessageAsync(user, message);
			// När man tar emot ett meddelande så skickas det ut till Alla användare/klienter. Även den som skickade det. 
			await Clients.All.SendAsync("RecieveMessage", user, message);


		}
	}
}
