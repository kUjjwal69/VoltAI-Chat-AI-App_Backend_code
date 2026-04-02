namespace WebApplication1.Models
{ 
        public class AIChatMessage
        {
            public string Role { get; set; } = string.Empty;    // "user" or "assistant"
            public string Content { get; set; } = string.Empty;
        }

        public class ChatSession
        {
            public string SessionId { get; set; } = Guid.NewGuid().ToString();
            public List<AIChatMessage> Messages { get; set; } = new();
        }

        public class ChatRequest
        {
            public string? SessionId { get; set; }   // null = start new session
            public string UserMessage { get; set; } = string.Empty;
        }

        public class ChatResponse
        {
            public string SessionId { get; set; } = string.Empty;
            public string Reply { get; set; } = string.Empty;
        }
    }

