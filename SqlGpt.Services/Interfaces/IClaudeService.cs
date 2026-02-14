using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services.Interfaces
{
     public interface IClaudeService
    {
       public Task<string> GetResponseAsync(string userMessage, CancellationToken ct = default);
    }
}
