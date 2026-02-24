using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlGpt.Dto;
using SqlGpt.Services.Interfaces;
using System.Security.Claims;

namespace SqlGpt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatService _chatService;
        private IMessageService _messageService;

        public ChatController(IChatService chatService,IMessageService messageService)
        {
            this._chatService = chatService;
            this._messageService = messageService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(MessageRequestDto message)
        {
            string? userId = User?.Identity?.IsAuthenticated == true
            ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             : null;
            try
            {
                MessageResponseDto response = await _chatService.SendMessageAsync(message, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("userChats")]
        [Authorize]
        public async Task<IActionResult> UserChats() 
        {

            string? getUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (getUserId==null)
            {
                return Unauthorized();
            }

            List<MyChatDto> getChats = await _chatService.GetUserChatsAsync(getUserId);

            // това го премахва от тука за да подобря performance i pravq вместо това нов ендпойнт зареждащ месидижите
            //foreach (MyChatDto c in getChats) 
            //{
            //    string chatId = c.ChatId.ToString();
                
            //    c.oldMessages = await _messageService.GetChatMessagesByChatIdAsync(chatId);
            //}

            return Ok(getChats);
        }
        [HttpGet("{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetChatMessages(string chatId) 
        {
            
            MyChatDto chat = await _chatService.GetChatByChatId(chatId);
            if (chat==null)
            {
                return NotFound();
            }
            string? getUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (chat.AppUserId !=getUserId)
            {
                return Unauthorized();
            }
            List<ChatMessagesDto> messages = await _messageService.GetChatMessagesByChatIdAsync(chatId);
            chat.oldMessages = messages;
            
            return Ok(chat);
        }
    }
}
