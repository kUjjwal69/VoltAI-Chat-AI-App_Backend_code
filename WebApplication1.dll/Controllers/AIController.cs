using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interface;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIServices _aIServices;
        private readonly IChatHistoryService _chatHistory;

        public AIController(IAIServices aIServices, IChatHistoryService chatHistory)
        {
            _aIServices = aIServices;
            _chatHistory = chatHistory;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
                return BadRequest("Message cannot be empty.");

            // Get or create session in DB
            var session = await _chatHistory.GetOrCreateSessionAsync(request.SessionId);

            // Save user message to DB
            await _chatHistory.AddMessageAsync(session.SessionId, "user", request.UserMessage);

            // Get full history from DB and send to Groq
            var messages = await _chatHistory.GetMessagesAsync(session.SessionId);
            var reply = await _aIServices.ChatAsync(messages);

            // Save AI reply to DB
            await _chatHistory.AddMessageAsync(session.SessionId, "assistant", reply);

            return Ok(new ChatResponse
            {
                SessionId = session.SessionId,
                Reply = reply
            });
        }

        [HttpGet("ask")]
        public async Task<IActionResult> AskAI(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question cannot be empty.");

            var response = await _aIServices.AskAI(question);
            return Ok(new { reply = response });
        }

        [HttpDelete("chat/{sessionId}")]
        public async Task<IActionResult> ClearChat(string sessionId)
        {
            await _chatHistory.ClearSessionAsync(sessionId);
            return Ok(new { message = "Session cleared." });
        }
    }
}