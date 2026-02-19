using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Infrastructure.InfrastructureModels
{
    public class ClaudeMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
