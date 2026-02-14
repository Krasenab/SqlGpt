using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Dto
{
    public class MessageRequestDto
    {
        public Guid? ChatId { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
