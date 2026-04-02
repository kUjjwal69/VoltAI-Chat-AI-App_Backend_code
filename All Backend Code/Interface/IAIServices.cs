using WebApplication1.Models;

namespace WebApplication1.Interface
{
    public interface IAIServices
    {
        Task<string> AskAI(string question);
        Task<string> ChatAsync(List<AIChatMessage> messages);
    }
}
