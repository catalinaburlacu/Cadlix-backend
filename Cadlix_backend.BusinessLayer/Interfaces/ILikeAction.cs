using Cadlix_backend.Domain.DTOs.Frontend;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface ILikeAction
{
    LikeStatusDto ToggleLike(int likerId, int likedUserId);
    LikeStatusDto GetLikeStatus(int likedUserId, int? currentUserId);
}
