using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.History;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly AppDbContext _context;

    public HistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HistoryData>> GetAllAsync()
    {
        return await _context.Histories.ToListAsync();
    }

    public async Task<HistoryData?> GetByIdAsync(int id)
    {
        return await _context.Histories.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<HistoryData> AddAsync(HistoryData entity)
    {
        await _context.Histories.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<HistoryData?> UpdateAsync(HistoryData entity)
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

        _context.Histories.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
