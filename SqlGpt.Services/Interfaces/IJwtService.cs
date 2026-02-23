using SqlGpt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(AppUser user);
    }
}
