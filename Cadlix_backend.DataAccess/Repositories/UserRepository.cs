using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserData>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<UserData?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<UserData> AddAsync(UserData entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserData?> UpdateAsync(UserData entity)
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

        _context.Users.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
