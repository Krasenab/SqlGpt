using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlGpt.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AppUserId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public AppUser? AppUser { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
