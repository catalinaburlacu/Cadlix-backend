using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Movie;
using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

public class MovieService : IMovieService
{
    private readonly AppDbContext _context;

    public MovieService()
    {
        _context = new AppDbContext();
    }

    public List<GetMovieDetailDTO> GetAllMovies()
    {
        var movies = _context.Movies
            .Include(m => m.Genres)
            .ToList();
        return movies.Select(movie => new GetMovieDetailDTO
        {
            Id = movie.Id,
            Title = movie.Title,
            Genres = movie.Genres?.Select(g => g.Name ?? string.Empty).ToList(),
            Country = movie.Country,
            Year = movie.Year,
            Description = movie.Description
        }).ToList();
    }

    public GetMovieByIdDTO? GetMovieById(int id)
    {
        var movie = _context.Movies
            .Include(m => m.Genres)
            .FirstOrDefault(m => m.Id == id);
        if (movie is null)
            return null;

        return new GetMovieByIdDTO
        {
            Title = movie.Title,
        };
    }

    public void CreateMovie(CreateMovieDTO createMovieDTO)
    {
        var genreList = _context.Categories
            .Where(c => c.Name != null && createMovieDTO.Genres != null && createMovieDTO.Genres.Contains(c.Name))
            .ToList();

        var movie = new MovieData
        {
            Title = createMovieDTO.Title,
            Genres = genreList.Any() ? genreList : new List<CategoryData>(),
            Country = createMovieDTO.Country,
            Year = createMovieDTO.Year,
            Description = createMovieDTO.Description,
        };

        _context.Movies.Add(movie);
        _context.SaveChanges();
    }
}
