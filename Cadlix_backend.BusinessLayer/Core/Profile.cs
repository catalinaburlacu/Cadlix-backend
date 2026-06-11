using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.History;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

public partial class FrontendActions
{
    public UserProfileDto? GetProfile(int userId)
    {
        var user = _context.Users.FirstOrDefault(entity => entity.Id == userId);
        if (user == null)
            return null;

        var watchListEntities = _context.Lists
            .Include(l => l.Film)
            .Where(item => item.UserId == userId)
            .ToList();

        var historyEntities = _context.Histories
            .Include(h => h.Movie)
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.WatchedAt)
            .ToList();

        var watchList = watchListEntities.Select(l =>
        {
            var dto = _mapper.Map<WatchListItemDto>(l);
            var movie = l.Film;
            if (movie != null)
            {
                dto.Poster = MediaUrl.Poster(movie.Poster) is { Length: >0 } p ? p : MediaUrl.Thumbnail(movie.Thumbnail);
                dto.Type = movie.Type ?? movie.Category ?? string.Empty;
                dto.Score = movie.Score;
            }
            return dto;
        }).ToList();

        var reviewCount = _context.Reviews.Count(r => r.UserId == userId);
        var actualTitlesWatched = historyEntities
            .Where(h => h.Movie != null)
            .Select(h => string.IsNullOrEmpty(h.Movie!.Series)
                ? $"movie:{h.MovieId}"
                : $"series:{h.Movie.Series}")
            .Distinct()
            .Count();
        var actualDaysOnSite = user.JoinedAt.HasValue
            ? (int)(DateTime.UtcNow - user.JoinedAt.Value).TotalDays
            : 0;

        // Score: match leaderboard formula exactly
        var scoreWatchHours = (historyEntities.Sum(h => (double?)h.ProgressPercentage) ?? 0) / 100.0;
        var scoreMoviesCount = _context.Histories
            .Count(h => h.UserId == userId && h.ProgressPercentage > 0);
        var scoreReviewCount = _context.Histories
            .Count(h => h.UserId == userId && h.UserRating.HasValue);
        var scoreLikesCount = user.LikesReceived;
        var computedScore = Math.Round(
            scoreWatchHours * 1.0 +
            scoreMoviesCount * 5.0 +
            scoreReviewCount * 2.0 +
            scoreLikesCount * 0.5, 3);

        return new UserProfileDto
        {
            Id = user.Id.ToString(),
            Role = user.Level.ToString().ToLowerInvariant(),
            Username = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Avatar = user.Avatar ?? string.Empty,
            Group = user.Group ?? "Member",
            Plan = user.Plan ?? "Free",
            Status = user.Status ?? "Online",
            Stats = new UserStatsDto
            {
                Score = computedScore,
                TitlesWatched = actualTitlesWatched,
                Comments = user.Comments,
                LikesGiven = user.LikesGiven,
                LikesReceived = user.LikesReceived,
                HoursWatched = (int)scoreWatchHours,
                AddedToList = watchListEntities.Count,
                DaysOnSite = actualDaysOnSite
            },
            WatchList = watchList,
            WatchHistory = _mapper.Map<List<WatchHistoryItemDto>>(historyEntities)
        };
    }
}
