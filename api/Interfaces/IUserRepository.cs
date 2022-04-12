using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IUserRepository
    {
         void Update(AppUser user);
         Task<bool> SaveAllAsync();
         Task<IEnumerable<AppUser>> GetUserAsync();
         Task<AppUser> GetUserByIdAsync(int id);
         Task<AppUser> GetUserByUsernameAsync(string username);
         Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
         Task<MemberDto> GetMemberAsync(string username);
    }
}