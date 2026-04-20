using System;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Services;

public class MovieService: IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<List<GetMovieDetailDTO>> GetAllMoviesAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        return movies.Select(movie => new GetMovieDetailDTO
        {
            Id = movie.Id,
            Title = movie.Title,
            Genre = movie.Genre,
            Country = movie.Country,
            ReleaseDate = movie.ReleaseDate,
            Rating = movie.Rating,
            Description = movie.Description
        }).ToList();
    }

    public async Task<GetMovieByIdDTO?> GetMovieByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie is null)
        {
            return null;
        }

        return new GetMovieByIdDTO
        {
            Title = movie.Title,
            // Link = movie.Link
        };
    }

    public async Task CreateMovieAsync(CreateMovieDTO createMovieDTO)
    {
        var movie = new Domain.Entities.Movie.MovieData
        {
            Title = createMovieDTO.Title,
            Genre = createMovieDTO.Genre,
            Country = createMovieDTO.Country,
            ReleaseDate = createMovieDTO.ReleaseDate,
            Rating = createMovieDTO.Rating,
            Description = createMovieDTO.Description,
            // Link = createMovieDTO.Link
        };

        await _movieRepository.AddAsync(movie);
    }
}
