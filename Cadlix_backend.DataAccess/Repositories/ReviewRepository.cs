using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<double> GetAverageRatingAsync(int userId)
    {
        var ratings = _context.Histories
            .AsNoTracking()
            .Where(history => history.UserId == userId && history.UserRating.HasValue)
            .Select(history => history.UserRating!.Value);

        return await ratings.AnyAsync()
            ? await ratings.AverageAsync()
            : 0;
    }

    public async Task<int> GetReviewCountAsync(int userId)
    {
        return await _context.Histories
            .AsNoTracking()
            .Where(history => history.UserId == userId && history.UserRating.HasValue)
            .CountAsync();
    }

    public Task<int> GetTotalLikesReceivedAsync(int userId)
    {
        return Task.FromResult(0);
    }
}
