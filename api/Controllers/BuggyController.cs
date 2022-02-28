using api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("not-found")]
        public ActionResult<object> GetNotFound(){
            var thing = _context.Users.Find(-1);

            if(thing == null) return new NotFoundObjectResult( new { message = "Not Found"});
            
            return new OkObjectResult(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<object> GetServerError(){
            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();
            
            return thingToReturn;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }
    }
}