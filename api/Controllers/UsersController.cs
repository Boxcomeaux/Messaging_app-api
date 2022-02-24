using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
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

        // api/users/{id}
        /*[HttpGet("{id}")]
        public async Task<memberDto> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<memberDto>(user);
        }*/
    }
}