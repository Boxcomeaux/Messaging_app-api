using System.Security.Cryptography;
using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register(RegisterDto registerDto)
        {
            //CHECK AND ADD USER TO MEMORY
            if (!String.IsNullOrEmpty(registerDto.Username) && !String.IsNullOrEmpty(registerDto.Password))
            {
                //CHECK IF USER EXISTS IN DATABASE
                if (await UserDoesNotExist(registerDto.Username))
                {
                    var user = _mapper.Map<AppUser>(registerDto);
                    //EXECUTE HMACSHA512 ALGORITHM
                    using var hmac = new HMACSHA512();

                    //CREATE USER OBJECT

                    user.UserName = registerDto.Username.Trim();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                    user.PasswordSalt = hmac.Key;


                    //ADD USER TO MEMORY
                    _context.Users.Add(user);

                    //ADD USER TO DATABASE
                    await _context.SaveChangesAsync();

                    //SEND RESULT TO CLIENT
                    return new UserDto{
                        Username = user.UserName,
                        Token = _tokenService.CreateToken(user),
                        KnownAs = user.KnownAs,
                        Gender = user.Gender
                    };
                }
                else
                {
                    return new { message = "The Username provided has been taken." };
                }
            }
            else
            {
                return new { message = "Please type a valid username and password." };
            }

            //SAVE USER
        }

        [HttpPost("login")]
        public async Task<object> Login(LoginDto loginDto)
        {
            try
            {
                //SEARCH FOR USER IN THE DATABASE
                var user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.Username.ToLower());

                //ERROR HANDLING FOR USER SEARCHED IN THE DATABASE
                if (user != null)
                {
                    //ADD PASSWORDSALT AS KEY fo DECRYPTION
                    using var hmac = new HMACSHA512(user.PasswordSalt);

                    //CONVERT PASSWORD TO HMACSHA512
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != user.PasswordHash[i]) return new { message = "Incorrect password associated with username." };
                    }
                    
                    string token = _tokenService.CreateToken(user);
                   // HttpContext.Session.SetString("XSRF_Auth", token);   
                                var cookieOptions = new CookieOptions()
                                {
                                    IsEssential = true,
                                    Expires = DateTime.Now.AddDays(1),
                                    Secure = true,
                                    HttpOnly = true,
                                    SameSite = SameSiteMode.Lax
                                };
                                Response.Cookies.Append("XSRF_Auth", token, cookieOptions);
                             
                    return new UserDto{
                        Username = user.UserName,
                        Token = token,
                        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                        KnownAs = user.KnownAs,
                        Gender = user.Gender
                    };
                }
                else
                {
                    return new { message = "Username not found" };
                }
            }
            catch (Exception e)
            {
                return new { message = e.Message };
            }
        }

        private async Task<bool> UserDoesNotExist(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower()) == true ? false : true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}