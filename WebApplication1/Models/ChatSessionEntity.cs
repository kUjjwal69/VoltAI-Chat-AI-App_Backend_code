using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ChatSessionEntity
    {
        [Key]
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessageEntity> Messages { get; set; } = new();
    }

    public class ChatMessageEntity
    {
        [Key]
        public int Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;       // "user" or "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ChatSessionEntity Session { get; set; } = null!;
    }
}