using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.BusinessLogic.Exceptions;
using User = Cadlix_backend.Domain.Entities.User.UserData;

namespace Cadlix_backend.BusinessLogic.Services;

public class LeaderboardService
{
    private readonly IUserRepository _userRepo;
    private readonly IWatchHistoryRepository _watchRepo;
    private readonly IReviewRepository _reviewRepo;

    private const double PtsPerHour = 1.0;
    private const double PtsPerMovie = 5.0;
    private const double PtsPerReview = 2.0;
    private const double PtsPerLike = 0.5;

    public LeaderboardService(
        IUserRepository userRepo,
        IWatchHistoryRepository watchRepo,
        IReviewRepository reviewRepo)
    {
        _userRepo = userRepo;
        _watchRepo = watchRepo;
        _reviewRepo = reviewRepo;
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetTopUsersAsync(int count = 100)
    {
        var users = await _userRepo.GetAllAsync();
        var entries = new List<LeaderboardEntryDto>();

        foreach (var user in users)
        {
            var entry = await BuildEntryAsync(user);
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

    public async Task<LeaderboardEntryDto> GetUserRankAsync(int userId)
    {
        var all = await GetTopUsersAsync(int.MaxValue);
        return all.FirstOrDefault(e => e.UserId == userId)
            ?? throw new NotFoundException("Userul nu a fost gasit in leaderboard.");
    }

    public async Task<double> CalculateScoreAsync(int userId)
    {
        var watchHours = await _watchRepo.GetTotalWatchHoursAsync(userId);
        var moviesCount = await _watchRepo.GetMoviesWatchedCountAsync(userId);
        var reviewsCount = await _reviewRepo.GetReviewCountAsync(userId);
        var likesCount = await _reviewRepo.GetTotalLikesReceivedAsync(userId);

        return (watchHours * PtsPerHour)
             + (moviesCount * PtsPerMovie)
             + (reviewsCount * PtsPerReview)
             + (likesCount * PtsPerLike);
    }

    private async Task<LeaderboardEntryDto> BuildEntryAsync(User user)
    {
        var watchHours = await _watchRepo.GetTotalWatchHoursAsync(user.Id);
        var moviesCount = await _watchRepo.GetMoviesWatchedCountAsync(user.Id);
        var episodesCount = await _watchRepo.GetEpisodesWatchedCountAsync(user.Id);
        var avgRating = await _reviewRepo.GetAverageRatingAsync(user.Id);
        var reviewsCount = await _reviewRepo.GetReviewCountAsync(user.Id);
        var likesCount = await _reviewRepo.GetTotalLikesReceivedAsync(user.Id);

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
}
