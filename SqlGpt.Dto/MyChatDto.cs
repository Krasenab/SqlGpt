using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Dto
{
    public class MyChatDto
    {
        public Guid ChatId { get; set; }
        public string AppUserId { get; set; }
        public List<ChatMessagesDto> oldMessages { get; set; }
     
    }
}
