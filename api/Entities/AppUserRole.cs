using api.Models;

namespace api.Entities
{
    public class AppUserRole
    {
        public AppUser User{ get; set; }

        public AppRole Role { get; set; }
    }
}