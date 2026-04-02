using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interface;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly AppDbContext _db;

        public ChatHistoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ChatSessionEntity> GetOrCreateSessionAsync(string? sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                var existing = await _db.ChatSessions
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (existing != null) return existing;
            }

            // Create new session
            var session = new ChatSessionEntity();
            _db.ChatSessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task AddMessageAsync(string sessionId, string role, string content)
        {
            var message = new ChatMessageEntity
            {
                SessionId = sessionId,
                Role = role,
                Content = content
            };

            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AIChatMessage>> GetMessagesAsync(string sessionId)
        {
            return await _db.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new AIChatMessage
                {
                    Role = m.Role,
                    Content = m.Content
                })
                .ToListAsync();
        }

        public async Task ClearSessionAsync(string sessionId)
        {
            var messages = _db.ChatMessages.Where(m => m.SessionId == sessionId);
            _db.ChatMessages.RemoveRange(messages);

            var session = await _db.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null)
                _db.ChatSessions.Remove(session);

            await _db.SaveChangesAsync();
        }
    }
}