using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;
using System.Collections.Generic;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly DBserviceChat dbService;

        public ChatController()
        {
            dbService = new DBserviceChat();
        }

        [HttpPost("MarkAsRead/{fromUserId}/{toUserId}")]
        public IActionResult MarkAsRead(int fromUserId, int toUserId)
        {
            dbService.MarkMessagesAsRead(fromUserId, toUserId);
            return Ok();
        }

        [HttpPost("SaveMessage")]
        public IActionResult SaveMessage([FromBody] ChatMessageDto messageDto)
        {
            ChatMessage message = new ChatMessage
            {
                FromUserId = messageDto.FromUserId,
                ToUserId = messageDto.ToUserId,
                Content = messageDto.Content,
                SentAt = DateTime.UtcNow
            };

            dbService.AddChatMessage(message);
            return Ok();
        }

        [HttpGet("GetMessages/{user1}/{user2}")]
        public IActionResult GetMessages(int user1, int user2)
        {
            List<ChatMessage> messages = dbService.GetChatMessages(user1, user2);
            return Ok(messages);
        }
        [HttpGet("GetChatList/{userId}")]
        public IActionResult GetChatList(int userId)
        {
            List<ChatListItem> chatList = dbService.GetUserChatList(userId);
            return Ok(chatList);
        }

    }
}
