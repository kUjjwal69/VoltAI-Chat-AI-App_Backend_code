using WebApplication1.Models;

namespace WebApplication1.Interface
{
    public interface IChatHistoryService
    {
        Task<ChatSessionEntity> GetOrCreateSessionAsync(string? sessionId);
        Task AddMessageAsync(string sessionId, string role, string content);
        Task<List<AIChatMessage>> GetMessagesAsync(string sessionId);
        Task ClearSessionAsync(string sessionId);
    }
    }

