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

        public async Task<MyChatDto> GetChatByChatId(string chatId)
        {
            MyChatDto? c = await _db.Chats.Where(c => c.Id.ToString() == chatId).Select(ch => new MyChatDto
            {
                AppUserId = ch.AppUserId.ToString(),
                ChatId = ch.Id,
                

            }).FirstOrDefaultAsync();

            return c;
        }

        public async Task<List<MyChatDto>> GetUserChatsAsync(string userId)
        {
            List<MyChatDto> chats = await _db.Chats.Where(au=>au.AppUserId.ToString()==userId)
                .Select(c=>new MyChatDto 
                {
                    AppUserId = c.AppUserId.ToString(),
                    ChatId = c.Id
                  
                })
                .ToListAsync();

            return chats;

        }

        // create message za ai i s claude servica vzimam otgowora ot AI , добавкам и анонимен режим където нищо не записваме в базата
        public async Task<MessageResponseDto> SendMessageAsync(MessageRequestDto inputDto,string? userId)
        {

            
            if (userId == null) 
            {
                // suzdavam nova istoriq na ne lognat potrebitel
                List<ClaudeMessage> tempHistoryData = new List<ClaudeMessage>();
                
                ClaudeMessage msg = new ClaudeMessage();
                msg.Content = inputDto.Message;
                msg.Role = "user";
                tempHistoryData.Add(msg);

                var response = await _claudeService.GetResponseAsync(tempHistoryData);


                MessageResponseDto responseDto = new MessageResponseDto() 
                {
                    AiMessage = response,
                    AppUserMessage = inputDto.Message


                };
               
                return responseDto;
            }


            Chat? c;
            if (inputDto.ChatId == null)
            {
                c = new Chat()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    AppUserId = Guid.Parse(userId)
                };

                _db.Chats.Add(c);
            }
            else
            {
                c = await _db.Chats.FirstOrDefaultAsync(x => x.Id == inputDto.ChatId && x.AppUserId.ToString() == userId);

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
            // bez mokvane realno kak se deistva
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
