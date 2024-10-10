using LeChat.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeChat.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
		// Här läggs de tabeller till i databasen som behövs utöver de som kom med Identity Framework
		public DbSet<ChatMessage> ChatMessages { get; set; }
		public DbSet<Conversation> Conversations { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Konfigurera relationer mellan tabellerna
			modelBuilder.Entity<ChatMessage>()
				.HasOne(cm => cm.User)
				.WithMany(u => u.Messages)
				.HasForeignKey(cm => cm.UserId);

			modelBuilder.Entity<ChatMessage>()
				.HasOne(cm => cm.Conversation)
				.WithMany(c => c.Messages)
				.HasForeignKey(cm => cm.ConversationId);
		}
	}
}
