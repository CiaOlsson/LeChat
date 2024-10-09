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
			var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
			var conversation = await _context.Conversations.SingleOrDefaultAsync(c => c.Id == 3);
			var chatMessage = new ChatMessage
			{
				UserId = user.Id,
				User = user,
				Message = message,
				CreatedDate = DateTime.UtcNow,
				ConversationId = 3,
				Conversation = conversation

			};

			_context.ChatMessages.Add(chatMessage);
			await _context.SaveChangesAsync();

		

		}

		public async Task<List<ChatMessage>> GetMessagesAsync()
		{
			return await _context.ChatMessages.ToListAsync();
		}
	}
}
