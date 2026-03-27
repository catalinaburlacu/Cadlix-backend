using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class ListsRepository : IListsRepository
{
    private readonly AppDbContext _context;

    public ListsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ListsData>> GetAllAsync()
    {
        return await _context.Lists.ToListAsync();
    }

    public async Task<ListsData?> GetByIdAsync(int id)
    {
        return await _context.Lists.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<ListsData> AddAsync(ListsData entity)
    {
        await _context.Lists.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ListsData?> UpdateAsync(ListsData entity)
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

        _context.Lists.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
