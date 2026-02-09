using Microsoft.AspNetCore.Identity;

namespace SqlGpt.Models
{
    public class AppUser: IdentityUser<Guid>
    {
        public List<Chat> Chats { get; set; } = new List<Chat>();
    }
}
