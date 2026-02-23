using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string getCurrentUserId() 
        {
           // string? userId = User?.Identity?.IsAuthenticated == true
           //? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
           //: null;

            return null;
        }
    }
}
