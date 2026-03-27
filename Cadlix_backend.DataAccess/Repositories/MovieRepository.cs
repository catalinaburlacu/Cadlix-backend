using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;

    public MovieRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MovieData>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task<MovieData?> GetByIdAsync(int id)
    {
        return await _context.Movies.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<MovieData> AddAsync(MovieData entity)
    {
        await _context.Movies.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<MovieData?> UpdateAsync(MovieData entity)
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

        _context.Movies.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
