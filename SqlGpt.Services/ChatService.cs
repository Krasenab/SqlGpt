using Microsoft.EntityFrameworkCore;
using SqlGpt.Data;
using SqlGpt.Dto;
using SqlGpt.Infrastructure.InfrastructureModels;
using SqlGpt.Models;
using SqlGpt.Services.Interfaces;

namespace SqlGpt.Services
{
    public class ChatService : IChatService
    {
        private SqlGptDbContext _db;
        private IClaudeService _claudeService;
        public ChatService(SqlGptDbContext dbContext,IClaudeService claudeService) 
        {
            this._db = dbContext;
            this._claudeService = claudeService;
        }
        // create message and rsponse from AI  ( realno tova mi e chat razgovora)
        public async Task<MessageResponseDto> SendMessageAsync(MessageRequestDto inputDto)
        {

            Chat? c;

            if (inputDto.ChatId == null)
            {
                c = new Chat()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                };

                _db.Chats.Add(c);
            }
            else
            {
                c = await _db.Chats.FirstOrDefaultAsync(x => x.Id == inputDto.ChatId);

                if (c == null)
                    throw new Exception("Chat not found");
            }




            Message m = new Message()
            {
                ChatId = c.Id,
                Content = inputDto.Message,
                CreatedAt = DateTime.UtcNow,
                IsFromUser = true
                
            };

            _db.Messages.Add(m);
            await _db.SaveChangesAsync();
            // vzimane na istoriqta
            List<Message> getMessage = await _db.Messages.Where(x => x.ChatId == c.Id).OrderByDescending(x => x.CreatedAt).Take(10).OrderBy(x => x.CreatedAt).ToListAsync();
            List<ClaudeMessage> history =  getMessage.Select(x => new ClaudeMessage
            {
                Role = x.IsFromUser ? "user" : "assistant",
                Content = x.Content
            })
            .ToList();

            // vremenno mokvam
            // string fakeAi = "How can I help human. I am AI";

            var aiResponse = await _claudeService.GetResponseAsync(history); // vzimane na responsa ot claude service
            Message aiResponseMessage = new Message() 
            {
                Content = aiResponse,
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
