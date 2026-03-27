using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryData>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<CategoryData?> GetByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<CategoryData> AddAsync(CategoryData entity)
    {
        await _context.Categories.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<CategoryData?> UpdateAsync(CategoryData entity)
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

        _context.Categories.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
