using Microsoft.EntityFrameworkCore;
using SqlGpt.Data;
using SqlGpt.Dto;
using SqlGpt.Models;
using SqlGpt.Services.Interfaces;

namespace SqlGpt.Services
{
    public class ChatService : IChatService
    {
        private SqlGptDbContext _db;

        public ChatService(SqlGptDbContext dbContext) 
        {
            this._db = dbContext;
        }
        // create-vam messages ( realno trqbva da se kazva createMessage !)
        public async Task<MessageResponseDto> SendMessageAsync(MessageRequestDto inputDto)
        {
            Chat c;
            if (inputDto.ChatId == null)
            {
                c = new Chat()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                };

                _db.Chats.Add(c);
            }

               c = await _db.Chats.FirstOrDefaultAsync(c => c.Id == inputDto.ChatId);

               
        

            Message m = new Message()
            {
                ChatId = inputDto.ChatId,
                Content = inputDto.Message,
                CreatedAt = DateTime.UtcNow,
                IsFromUser = true
                
            };

            _db.Messages.Add(m);

            // vremenno mokvam
            string fakeAi = "How can I help human. I am AI";
            Message aiResponseMessage = new Message() 
            {
                Content = fakeAi,
                CreatedAt = DateTime.UtcNow,
                ChatId = c.Id,
                IsFromUser = false
            };

            _db.Messages.Add(aiResponseMessage);
            await _db.SaveChangesAsync();
            return new MessageResponseDto()
            {
                ChatId = c.Id,
                AiMessage = aiResponseMessage.Content,
                AppUserMessage = inputDto.Message
            };
            
        }
    }
}
