using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Movie;
using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

public class MovieAction
{
    private readonly AppDbContext _context;

    public MovieAction()
    {
        _context = new AppDbContext();
    }

    public List<GetMovieDetailDTO> GetAllMoviesExecution()
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
            Rating = movie.Rating,
            Description = movie.Description
        }).ToList();
    }

    public GetMovieByIdDTO? GetMovieByIdExecution(int id)
    {
        var movie = _context.Movies
            .Include(m => m.Genres)
            .FirstOrDefault(m => m.Id == id);
        if (movie is null)
            return null;

        return new GetMovieByIdDTO
        {
            Title = movie.Title,
            Link = movie.VideoSource
        };
    }

    public int CreateMovieExecution(CreateMovieDTO createMovieDTO)
    {
        var genreList = _context.Categories
            .Where(c => createMovieDTO.Genres.Contains(c.Name))
            .ToList();

        var movie = new MovieData
        {
            Title = createMovieDTO.Title,
            Genres = genreList.Any() ? genreList : new List<CategoryData>(),
            Country = createMovieDTO.Country,
            Year = createMovieDTO.Year,
            Rating = createMovieDTO.Rating,
            Description = createMovieDTO.Description,
            VideoSource = createMovieDTO.VideoSource
        };

        _context.Movies.Add(movie);
        _context.SaveChanges();
        return movie.Id;
    }
}
