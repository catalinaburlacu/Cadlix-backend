using System.Linq;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.BusinessLayer.Core;

public class LikeActions : ILikeAction
{
    private readonly AppDbContext _context;

    public LikeActions()
    {
        _context = new AppDbContext();
    }

    public LikeStatusDto ToggleLike(int likerId, int likedUserId)
    {
        if (likerId == likedUserId)
            return GetLikeStatus(likedUserId, likerId);

        var existing = _context.UserLikes
            .FirstOrDefault(ul => ul.LikerId == likerId && ul.LikedUserId == likedUserId);

        if (existing != null)
        {
            _context.UserLikes.Remove(existing);
            DecrementCounts(likerId, likedUserId);
        }
        else
        {
            _context.UserLikes.Add(new UserLikeData
            {
                LikerId = likerId,
                LikedUserId = likedUserId,
                CreatedAt = System.DateTime.UtcNow
            });
            IncrementCounts(likerId, likedUserId);
        }

        _context.SaveChanges();
        return GetLikeStatus(likedUserId, likerId);
    }

    public LikeStatusDto GetLikeStatus(int likedUserId, int? currentUserId)
    {
        return new LikeStatusDto
        {
            LikeCount = _context.UserLikes.Count(ul => ul.LikedUserId == likedUserId),
            IsLikedByCurrentUser = currentUserId.HasValue
                && _context.UserLikes.Any(ul => ul.LikerId == currentUserId.Value && ul.LikedUserId == likedUserId)
        };
    }

    private void IncrementCounts(int likerId, int likedUserId)
    {
        var liker = _context.Users.Find(likerId);
        var liked = _context.Users.Find(likedUserId);

        if (liker != null) liker.LikesGiven++;
        if (liked != null) liked.LikesReceived++;
    }

    private void DecrementCounts(int likerId, int likedUserId)
    {
        var liker = _context.Users.Find(likerId);
        var liked = _context.Users.Find(likedUserId);

        if (liker != null && liker.LikesGiven > 0) liker.LikesGiven--;
        if (liked != null && liked.LikesReceived > 0) liked.LikesReceived--;
    }
}
