using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;

        }

        /*Get All Users*/
        [HttpGet]
        public async Task<IEnumerable<AppUser>> GetUsers(){
            var users = await _context.Users.ToListAsync();
            return users;
        }

        // api/users/{id}
        [HttpGet("{id}")]
        public async Task<object> GetUser(int id){
            var users = await _context.Users.Where(o => o.Id == id).FirstOrDefaultAsync();
            return users != null ? users : new {message = "User Not Found"};
        }
    }
}