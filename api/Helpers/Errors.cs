using Microsoft.AspNetCore.Mvc;

namespace api.Helpers
{
    public class Errors
    {
        public static ActionResult OkWithStatus(int statusCode, string message) {
            return new OkObjectResult( new {statusCode, message});
        }
    }
}