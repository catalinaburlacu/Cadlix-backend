using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Entities.Leaderboard;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly AppDbContext _context;

    public LeaderboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaderboardData>> GetAllAsync()
    {
        return await _context.Leaderboards.ToListAsync();
    }

    public async Task<LeaderboardData?> GetByIdAsync(int id)
    {
        return await _context.Leaderboards.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<LeaderboardData> AddAsync(LeaderboardData entity)
    {
        await _context.Leaderboards.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<LeaderboardData?> UpdateAsync(LeaderboardData entity)
    {
        var existing = await GetByIdAsync(entity.Id);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Leaderboards.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetTopUsersAsync(int count = 100)
    {
        if (count <= 0)
        {
            return Enumerable.Empty<LeaderboardEntryDto>();
        }

        var topEntries = await _context.Leaderboards
            .AsNoTracking()
            .OrderByDescending(entity => entity.Score)
            .ThenBy(entity => entity.Id)
            .Take(count)
            .ToListAsync();

        return topEntries
            .Select((entity, index) => MapToDto(entity, index + 1))
            .ToList();
    }

    public async Task<LeaderboardEntryDto> GetUserRankAsync(int userId)
    {
        var entry = await _context.Leaderboards
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == userId);

        if (entry is null)
        {
            throw new InvalidOperationException($"Leaderboard entry for user ID {userId} was not found.");
        }

        var rank = await _context.Leaderboards.CountAsync(entity =>
            entity.Score > entry.Score
            || (entity.Score == entry.Score && entity.Id < entry.Id));

        return MapToDto(entry, rank + 1);
    }

    public async Task<double> CalculateScoreAsync(int userId)
    {
        var entry = await _context.Leaderboards
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == userId);

        if (entry is null)
        {
            throw new InvalidOperationException($"Leaderboard entry for user ID {userId} was not found.");
        }

        return entry.Score;
    }

    private static LeaderboardEntryDto MapToDto(LeaderboardData entity, int rank)
    {
        return new LeaderboardEntryDto
        {
            UserId = entity.Id,
            Username = entity.Username ?? string.Empty,
            Score = entity.Score,
            Rank = rank,
            RankChange = 0
        };
    }
}
