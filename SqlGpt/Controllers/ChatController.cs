using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlGpt.Dto;
using SqlGpt.Services.Interfaces;

namespace SqlGpt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatService _chatService;

        public ChatController(IChatService chatService) 
        {
            this._chatService = chatService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(MessageRequestDto message) 
        {
            
            try
            {
                MessageResponseDto response = await _chatService.SendMessageAsync(message);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
