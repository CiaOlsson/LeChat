using LeChat.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LeChat.Data.Services
{
	public class ChatMessageService
	{
		private readonly ApplicationDbContext _context;

		public ChatMessageService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task SaveMessageAsync(string username, string message)
		{
			// först hämtas användaren så att jag kan lägga till den och UserId i ChatMessage-objektet
			var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

			//Detsamma gäller för Conversation. 
			var conversation = await _context.Conversations.SingleOrDefaultAsync(c => c.Id == 3);

			// Skapa ett nytt ChatMessage och sätt värden på dess properties. 
			var chatMessage = new ChatMessage
			{
				UserId = user.Id,
				User = user,
				Message = message,
				CreatedDate = DateTime.UtcNow,
				ConversationId = 3,
				Conversation = conversation

			};

			// Slutligen läggs det till i databasen asynkront
			await _context.ChatMessages.AddAsync(chatMessage);
			await _context.SaveChangesAsync();

		

		}

		public async Task<List<ChatMessage>> GetMessagesAsync()
		{
			//Hämta alla meddelanden från databasen för att kunna visa historik.
			//Denna funktion har jag inte helt implementerat ännu. 
			return await _context.ChatMessages.ToListAsync();
		}
	}
}
