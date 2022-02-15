using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class RegisterDto
    {
        [Required]
        public string Username {get; set;}

        [Required]
        public string Password {get; set;}

    }
}