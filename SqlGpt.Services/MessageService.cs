using Microsoft.EntityFrameworkCore;
using SqlGpt.Data;
using SqlGpt.Dto;
using SqlGpt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services
{
    public class MessageService : IMessageService
    {
        private SqlGptDbContext _db;
        public MessageService(SqlGptDbContext dbContext) 
        {
                this._db = dbContext;   
        }
        public async Task<List<ChatMessagesDto>> GetChatMessagesByChatIdAsync(string chatId)
        {
            List<ChatMessagesDto> chatMessagesDto = await _db.Messages.Where(c => c.ChatId.ToString() == chatId).Select(x => new ChatMessagesDto
            {
                Content = x.Content,
                isFormUser = x.IsFromUser
                
            }).ToListAsync();
            return chatMessagesDto;
        }
    }
}
