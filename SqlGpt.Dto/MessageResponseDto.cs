using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Dto
{
    public class MessageResponseDto
    {
        public Guid ChatId { get; set; }

        public string AppUserMessage { get; set; }

        public string AiMessage { get; set; }

    }
}
