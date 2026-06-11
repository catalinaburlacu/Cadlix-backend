using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs;
using User = Cadlix_backend.Domain.Entities.User.UserData;

namespace Cadlix_backend.BusinessLayer.Core;

public class LeaderboardActions : ILeaderboardAction
{
    private readonly AppDbContext _context;

    private const double PtsPerHour = 1.0;
    private const double PtsPerMovie = 5.0;
    private const double PtsPerReview = 2.0;
    private const double PtsPerLike = 0.5;

    public LeaderboardActions()
    {
        _context = new AppDbContext();
    }

    public IEnumerable<LeaderboardEntryDto> GetTopUsers(int count = 100)
    {
        var users = _context.Users.ToList();
        var entries = new List<LeaderboardEntryDto>();

        foreach (var user in users)
        {
            var entry = BuildEntry(user);
            entries.Add(entry);
        }

        var ranked = entries
            .OrderByDescending(e => e.Score)
            .Take(count)
            .ToList();

        for (int i = 0; i < ranked.Count; i++)
        {
            ranked[i].Rank = i + 1;
        }

        return ranked;
    }

    public LeaderboardEntryDto? GetUserRank(int userId)
    {
        var all = GetTopUsers(int.MaxValue);
        return all.FirstOrDefault(e => e.UserId == userId);
    }

    public double CalculateScore(int userId)
    {
        var watchHours = GetTotalWatchHours(userId);
        var moviesCount = GetMoviesWatchedCount(userId);
        var reviewsCount = GetReviewCount(userId);
        var likesCount = GetTotalLikesReceived(userId);

        return (watchHours * PtsPerHour)
             + (moviesCount * PtsPerMovie)
             + (reviewsCount * PtsPerReview)
             + (likesCount * PtsPerLike);
    }

    private LeaderboardEntryDto BuildEntry(User user)
    {
        var watchHours = GetTotalWatchHours(user.Id);
        var moviesCount = GetMoviesWatchedCount(user.Id);
        var episodesCount = GetEpisodesWatchedCount(user.Id);
        var avgRating = GetAverageRating(user.Id);
        var reviewsCount = GetReviewCount(user.Id);
        var likesCount = GetTotalLikesReceived(user.Id);

        var score = (watchHours * PtsPerHour)
                  + (moviesCount * PtsPerMovie)
                  + (reviewsCount * PtsPerReview)
                  + (likesCount * PtsPerLike);

        return new LeaderboardEntryDto
        {
            UserId = user.Id,
            Username = user.Name ?? string.Empty,
            AvatarUrl = string.Empty,
            Country = string.Empty,
            WatchTimeHours = watchHours,
            MoviesWatched = moviesCount,
            EpisodesWatched = episodesCount,
            AverageRating = avgRating,
            ReviewsWritten = reviewsCount,
            Score = Math.Round(score, 3)
        };
    }

    private double GetTotalWatchHours(int userId)
    {
        var totalProgress = _context.Histories
            .Where(history => history.UserId == userId)
            .Sum(history => (double?)history.ProgressPercentage) ?? 0;

        return totalProgress / 100.0;
    }

    private int GetMoviesWatchedCount(int userId)
    {
        return _context.Histories
            .Where(history => history.UserId == userId && history.ProgressPercentage > 0)
            .Select(history => history.MovieId)
            .Distinct()
            .Count();
    }

    private int GetEpisodesWatchedCount(int userId)
    {
        return _context.Histories
            .Count(history => history.UserId == userId
                && !string.IsNullOrWhiteSpace(history.Episode)
                && history.Episode != "-");
    }

    private double GetAverageRating(int userId)
    {
        var ratings = _context.Histories
            .Where(history => history.UserId == userId && history.UserRating.HasValue)
            .Select(history => history.UserRating!.Value)
            .ToList();

        return ratings.Count == 0 ? 0 : ratings.Average();
    }

    private int GetReviewCount(int userId)
    {
        return _context.Histories
            .Count(history => history.UserId == userId && history.UserRating.HasValue);
    }

    private int GetTotalLikesReceived(int userId)
    {
        return _context.UserLikes.Count(ul => ul.LikedUserId == userId);
    }
}
