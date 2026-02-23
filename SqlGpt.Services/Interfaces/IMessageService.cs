using SqlGpt.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<List<ChatMessagesDto>> GetChatMessagesByChatIdAsync(string chatId);
    }
}
