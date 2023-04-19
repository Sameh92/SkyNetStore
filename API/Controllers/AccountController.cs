using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private readonly UserManager<AppUserSamRan> _userManager;
        private readonly SignInManager<AppUserSamRan> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUserSamRan> userManager,SignInManager<AppUserSamRan> signInManager,
            ITokenService tokenService,IMapper mapper)
        {
           _userManager = userManager;
           _signInManager = signInManager;
           _tokenService = tokenService;
            _mapper = mapper;
        }
    
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));


            // Another way to check email and password using _userManger
            //if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            //{
            //    return Unauthorized(new ApiResponse(401));
            //}
            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegiserDto regiserDto)
        {
            // inside the same controller we can deal with onther endpoint as a function 
            if(CheckEmailExistAsync(regiserDto.Email).Result.Value) // Result.Value because of async
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse { Errors = new[] { "Email address is in use" } });
            }
            var user = new AppUserSamRan
            {
                DisplayName = regiserDto.DisplayName,
                Email = regiserDto.Email,
                UserName = regiserDto.Email
            };
            var result = await _userManager.CreateAsync(user,regiserDto.Password);
            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email,
            };
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
          

            var user = await _userManager.FindByEmailFromClaimsPrincipal(HttpContext.User);
            //var test = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x=>x.DisplayName=="sameh") not used because we
            //dont need to use DbContext here;

            // or you can access the claims principal Directly now like below:
            //var user = await _userManager.FindByEmailFromClaimsPrincipal(User);
            // Another method to get  email below
            //var email = User.FindFirstValue(ClaimTypes.Email);
            // Another method to get  email below
            // var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            //var user =await _userManager.FindByEmailAsync(email);

            // Another method to get user with out relay on email
            //var user = await _userManager.FindByNameAsync(User.Identity.Name); 
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email,
            };

        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserByEmailClaimsPrincipalWithAddressAsync(HttpContext.User);
          
            return _mapper.Map<Address,AddressDto>(user.Address);
        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindUserByEmailClaimsPrincipalWithAddressAsync(HttpContext.User);
            user.Address = _mapper.Map<AddressDto, Address>(address);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));
            return BadRequest("Proplem updateing the User");

        }

    }
}
