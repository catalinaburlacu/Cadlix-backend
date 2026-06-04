using System;
using System.Collections.Generic;
using System.Linq;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using UserEntity = Cadlix_backend.Domain.Entities.User.UserData;

namespace Cadlix_backend.BusinessLayer.Core;

public partial class FrontendActions
{
    public LeaderboardPayloadDto GetLeaderboardPage(int count = 100)
    {
        var users = _context.Users.ToList();
        var entries = users.Select(user => BuildLeaderboardEntry(user)).OrderByDescending(entry => entry.Score).Take(count).ToList();

        for (var index = 0; index < entries.Count; index++)
        {
            entries[index].Rank = index + 1;
        }

        return new LeaderboardPayloadDto
        {
            Filters = new LeaderboardFiltersDto
            {
                Time = new List<UiOptionDto>
                {
                    new() { Id = "all", Label = "All Time" },
                    new() { Id = "monthly", Label = "Monthly" },
                    new() { Id = "weekly", Label = "Weekly" }
                },
                Scope = new List<UiOptionDto>
                {
                    new() { Id = "global", Label = "Global" },
                    new() { Id = "country", Label = "Country" },
                    new() { Id = "friends", Label = "Friends" }
                }
            },
            Users = entries
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
        return _context.Histories.Count(history => history.UserId == userId && !string.IsNullOrWhiteSpace(history.Episode) && history.Episode != "-");
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
        return _context.Histories.Count(history => history.UserId == userId && history.UserRating.HasValue);
    }

    private static int GetTotalLikesReceived(int userId)
    {
        return 0;
    }

    private LeaderboardEntryDto BuildLeaderboardEntry(UserEntity user)
    {
        var watchHours = GetTotalWatchHours(user.Id);
        var moviesCount = GetMoviesWatchedCount(user.Id);
        var episodesCount = GetEpisodesWatchedCount(user.Id);
        var avgRating = GetAverageRating(user.Id);
        var reviewsCount = GetReviewCount(user.Id);
        var likesCount = GetTotalLikesReceived(user.Id);
        var score = (watchHours * 1.0) + (moviesCount * 5.0) + (reviewsCount * 2.0) + (likesCount * 0.5);

        return new LeaderboardEntryDto
        {
            UserId = user.Id,
            Username = user.Name ?? string.Empty,
            AvatarUrl = user.Avatar ?? string.Empty,
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
