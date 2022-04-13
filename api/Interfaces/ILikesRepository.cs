using api.Entities;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ILikesRepository
    {
         Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
         Task<AppUser> GetUserWithLikes(int userId);
         Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}