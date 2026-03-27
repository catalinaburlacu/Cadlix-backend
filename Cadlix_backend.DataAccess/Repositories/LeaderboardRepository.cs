using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
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
}
