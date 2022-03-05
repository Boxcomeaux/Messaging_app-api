using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api.Extensions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // api/users/
        [HttpGet]
        public async Task<IEnumerable<MemberDto>> GetUsers()
        {
            var users = await _userRepository.GetUserAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return usersToReturn;
        }

        // api/users/{username}
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<MemberDto> GetUserByUsername(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return new OkObjectResult(new { message = username + " was successfully updated." });

            return new OkObjectResult(new { message = "Failed to update user" });
        }

        [HttpPost("add-photo")]
        public async Task<object> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = _photoService.AddPhotoAsync(file);

            if(result.Result.Error != null) return new BadRequestObjectResult(new {message = result.Result.Error.Message});

            var photo = new Photo
            {
                Url = result.Result.SecureUrl.AbsoluteUri,
                PublicId = result.Result.PublicId
            };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;    
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                //return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo));
                return CreatedAtRoute("GetUser", new { username = user.UserName } ,_mapper.Map<PhotoDto>(photo));
            }

            return new BadRequestObjectResult(new {error = "Problem adding Photo"});
        }
    }
}