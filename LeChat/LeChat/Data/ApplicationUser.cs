using LeChat.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace LeChat.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
		// User ska ha en koppling till meddelanden
		public virtual ICollection<ChatMessage> Messages { get; set; }

	}

}
