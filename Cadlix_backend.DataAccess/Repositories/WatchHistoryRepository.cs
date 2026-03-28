using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class WatchHistoryRepository : IWatchHistoryRepository
{
    private readonly AppDbContext _context;

    public WatchHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<double> GetTotalWatchHoursAsync(int userId)
    {
        var totalProgress = await _context.Histories
            .AsNoTracking()
            .Where(history => history.UserId == userId)
            .SumAsync(history => (double?)history.ProgressPercentage) ?? 0;

        return totalProgress / 100.0;
    }

    public async Task<int> GetMoviesWatchedCountAsync(int userId)
    {
        return await _context.Histories
            .AsNoTracking()
            .Where(history => history.UserId == userId && history.ProgressPercentage > 0)
            .Select(history => history.MovieId)
            .Distinct()
            .CountAsync();
    }

    public Task<int> GetEpisodesWatchedCountAsync(int userId)
    {
        return Task.FromResult(0);
    }
}
