using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class MovieRepository : IMovieRepository
{
    public List<MovieData> GetAll()
    {
        using var db = new AppDbContext();
        return db.Movies.ToList();
    }

    public MovieData? GetById(int id)
    {
        using var db = new AppDbContext();
        return db.Movies.FirstOrDefault(entity => entity.Id == id);
    }

    public MovieData Add(MovieData entity)
    {
        using var db = new AppDbContext();
        db.Movies.Add(entity);
        db.SaveChanges();
        return entity;
    }

    public MovieData? Update(MovieData entity)
    {
        using var db = new AppDbContext();
        var existing = GetById(entity.Id);
        if (existing is null)
        {
            return null;
        }

        db.Entry(existing).CurrentValues.SetValues(entity);
        db.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        using var db = new AppDbContext();
        var existing = GetById(id);
        if (existing is null)
        {
            return false;
        }


        db.Movies.Remove(existing);
        db.SaveChanges();
        return true;
    }
}
