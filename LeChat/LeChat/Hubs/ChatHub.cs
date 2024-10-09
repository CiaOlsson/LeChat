using LeChat.Data;
using LeChat.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace LeChat.Hubs
{
	public class ChatHub : Hub
	{
		// här injectas ChatMessageService så att jag kan använda metoderna i den för att spara i och hämta från databasen.
		private readonly ChatMessageService _messageService;

		public ChatHub(ChatMessageService messageService)
		{
			_messageService = messageService;
		}
		public async Task SendMessage(string user, string message)
		{
			// Kallar på metoden som sparar meddelandet i databasen.
			await _messageService.SaveMessageAsync(user, message);

			// När man tar emot ett meddelande så skickas det ut till Alla användare/klienter. Även den som skickade det. 
			await Clients.All.SendAsync("RecieveMessage", user, message);


		}
	}
}
