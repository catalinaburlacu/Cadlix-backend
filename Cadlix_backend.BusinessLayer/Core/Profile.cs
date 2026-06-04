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
                dto.Score = movie.Score ?? movie.Rating;
            }
            return dto;
        }).ToList();

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
                Rating = user.Rating ?? 0,
                TitlesWatched = user.TitlesWatched,
                Comments = user.Comments,
                LikesGiven = user.LikesGiven,
                LikesReceived = user.LikesReceived,
                HoursWatched = user.HoursWatched,
                AddedToList = user.AddedToList,
                DaysOnSite = user.DaysOnSite
            },
            WatchList = watchList,
            WatchHistory = _mapper.Map<List<WatchHistoryItemDto>>(historyEntities)
        };
    }
}
