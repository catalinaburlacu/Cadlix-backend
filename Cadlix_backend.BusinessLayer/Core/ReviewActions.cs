using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Review;
using Cadlix_backend.Domain.Entities.Review;
using Cadlix_backend.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

public class ReviewActions : IReviewAction
{
    private readonly AppDbContext _context;

    public ReviewActions()
    {
        _context = new AppDbContext();
    }

    public ReviewDTO? GetById(int id)
    {
        var review = _context.Reviews
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == id);
        return review == null ? null : MapToDto(review);
    }

    public List<ReviewDTO> GetByMovie(int movieId, int? currentUserId = null)
    {
        var reviews = _context.Reviews
            .Include(r => r.User)
            .Where(r => r.MovieId == movieId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var likedIds = GetLikedReviewIds(reviews, currentUserId);
        return reviews.Select(r => MapToDto(r, likedIds.Contains(r.Id))).ToList();
    }

    public List<ReviewDTO> GetByUser(int userId, int? currentUserId = null)
    {
        var reviews = _context.Reviews
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var likedIds = GetLikedReviewIds(reviews, currentUserId);
        return reviews.Select(r => MapToDto(r, likedIds.Contains(r.Id))).ToList();
    }

    public ReviewDTO Create(int userId, CreateReviewDTO dto)
    {
        var existing = _context.Reviews
            .Include(r => r.User)
            .FirstOrDefault(r => r.UserId == userId && r.MovieId == dto.MovieId);

        if (existing != null)
        {
            existing.Rating = dto.Rating;
            existing.Text = dto.Text;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            RecalculateMovieScore(dto.MovieId);
            _context.SaveChanges();
            return MapToDto(existing);
        }

        var review = new ReviewData
        {
            UserId = userId,
            MovieId = dto.MovieId,
            Rating = dto.Rating,
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        _context.SaveChanges();

        _context.Entry(review).Reference(r => r.User).Load();
        RecalculateMovieScore(dto.MovieId);
        _context.SaveChanges();
        return MapToDto(review);
    }

    public ReviewDTO? Update(int id, int userId, CreateReviewDTO dto)
    {
        var review = _context.Reviews
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == id && r.UserId == userId);
        if (review == null) return null;

        review.Rating = dto.Rating;
        review.Text = dto.Text;
        review.UpdatedAt = DateTime.UtcNow;

        _context.SaveChanges();
        RecalculateMovieScore(review.MovieId);
        _context.SaveChanges();
        return MapToDto(review);
    }

    public bool Delete(int id, int userId)
    {
        var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
        if (review == null) return false;

        var movieId = review.MovieId;
        _context.Reviews.Remove(review);
        _context.SaveChanges();
        RecalculateMovieScore(movieId);
        _context.SaveChanges();
        return true;
    }

    public bool ToggleLike(int reviewId, int userId)
    {
        var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
        if (review == null) return false;

        var existing = _context.ReviewLikes
            .FirstOrDefault(rl => rl.ReviewId == reviewId && rl.UserId == userId);

        if (existing != null)
        {
            _context.ReviewLikes.Remove(existing);
            review.LikesCount = Math.Max(0, review.LikesCount - 1);

            var liker = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (liker != null) liker.LikesGiven = Math.Max(0, liker.LikesGiven - 1);

            var author = _context.Users.FirstOrDefault(u => u.Id == review.UserId);
            if (author != null) author.LikesReceived = Math.Max(0, author.LikesReceived - 1);

            _context.SaveChanges();
            return true;
        }

        var like = new ReviewLikeData
        {
            UserId = userId,
            ReviewId = reviewId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ReviewLikes.Add(like);
        review.LikesCount++;

        var likeLiker = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (likeLiker != null) likeLiker.LikesGiven++;

        var likeAuthor = _context.Users.FirstOrDefault(u => u.Id == review.UserId);
        if (likeAuthor != null) likeAuthor.LikesReceived++;

        _context.SaveChanges();
        return true;
    }

    private void RecalculateMovieScore(int movieId)
    {
        var sum = _context.Reviews
            .Where(r => r.MovieId == movieId)
            .Sum(r => (int?)r.Rating) ?? 0;

        var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie != null)
        {
            movie.Score = sum;
        }
    }

    private HashSet<int> GetLikedReviewIds(List<ReviewData> reviews, int? currentUserId)
    {
        if (!currentUserId.HasValue || reviews.Count == 0)
            return new HashSet<int>();

        var reviewIds = reviews.Select(r => r.Id).ToList();
        return _context.ReviewLikes
            .Where(rl => rl.UserId == currentUserId.Value && reviewIds.Contains(rl.ReviewId))
            .Select(rl => rl.ReviewId)
            .ToHashSet();
    }

    private static ReviewDTO MapToDto(ReviewData r, bool isLikedByCurrentUser = false)
    {
        return new ReviewDTO
        {
            Id = r.Id,
            UserId = r.UserId,
            Username = r.User?.Name ?? "Unknown",
            UserAvatar = r.User?.Avatar,
            MovieId = r.MovieId,
            Rating = r.Rating,
            Text = r.Text,
            LikesCount = r.LikesCount,
            IsLikedByCurrentUser = isLikedByCurrentUser,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
