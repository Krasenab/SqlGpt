using SqlGpt.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services.Interfaces
{
    public interface IChatService
    {
        public Task<MessageResponseDto> SendMessageAsync(MessageRequestDto inputDto,string? userId);
        public Task<List<MyChatDto>> GetUserChatsAsync(string userId);
        
        public Task<MyChatDto> GetChatByChatId(string chatId);
    }
}
