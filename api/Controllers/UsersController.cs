using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
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
        [HttpGet("{username}")]
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
        
            if(await _userRepository.SaveAllAsync()) return new OkObjectResult( new { message = username + " was successfully updated."});

            return new OkObjectResult( new { message = "Failed to update user"});
        }
    }
}