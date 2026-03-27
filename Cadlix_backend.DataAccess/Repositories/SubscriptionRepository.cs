using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionData>> GetAllAsync()
    {
        return await _context.Subscriptions.ToListAsync();
    }

    public async Task<SubscriptionData?> GetByIdAsync(int id)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<SubscriptionData> AddAsync(SubscriptionData entity)
    {
        await _context.Subscriptions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<SubscriptionData?> UpdateAsync(SubscriptionData entity)
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

        _context.Subscriptions.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
