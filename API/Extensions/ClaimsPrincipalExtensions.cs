using System.Linq;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal  user)
        {
            // note: this can now be simplified to use :
            // return user.FindFirstValue(ClaimTypes.Email)

            return user?.Claims?.FirstOrDefault(x=>x.Type== ClaimTypes.Email)?.Value;
        }
    }
}
