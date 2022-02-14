using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class RegisterDto
    {
        public string Username {get; set;} = String.Empty;

        public string Password {get; set;} = String.Empty;

    }
}