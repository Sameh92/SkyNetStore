using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class UserMangerExtensions
    {
        public static async Task<AppUserSamRan> FindUserByEmailClaimsPrincipalWithAddressAsync(this UserManager<AppUserSamRan> input,
            ClaimsPrincipal user)
        {

           // var email = user.FindFirstValue(ClaimTypes.Email); another way
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return await input.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
        }
        public static async Task<AppUserSamRan> FindByEmailFromClaimsPrincipal(this UserManager<AppUserSamRan> input,
          ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return await input.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
