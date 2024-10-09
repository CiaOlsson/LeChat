using System.ComponentModel.DataAnnotations;

namespace LeChat.Data.Entities
{
	public class ChatMessage
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string UserId { get; set; }
		[Required]
		public int ConversationId { get; set; }
		[Required]
		public string Message { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public virtual ApplicationUser User { get; set; }
		public virtual Conversation Conversation { get; set; }

	}
}
