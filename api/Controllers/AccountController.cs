using System.Security.Cryptography;
using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (String.IsNullOrEmpty(registerDto.Username) || String.IsNullOrEmpty(registerDto.Password)) return BadRequest("Invalid Username and/or Password");

            if (await UserDoesNotExist(registerDto.Username)) return BadRequest("User does not exist");

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.Trim();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.Username.ToLower());

            if (user == null) return BadRequest("Username Not Found");

            string token = await _tokenService.CreateToken(user);

            var cookieOptions = new CookieOptions()
            {
                IsEssential = true,
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("XSRF_Auth", token, cookieOptions);

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                Username = user.UserName,
                Token = token,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserDoesNotExist(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
        }
    }
}