using System;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Core;

public class MovieAction
{
    public readonly IMovieRepository _movieRepository;

    public MovieAction()
    {
        _movieRepository = new MovieRepository();
    }

    public List<GetMovieDetailDTO> GetAllMoviesExecution()
    {
        var movies = _movieRepository.GetAll();
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

    public GetMovieByIdDTO? GetMovieByIdExecution(int id)
    {
        var movie = _movieRepository.GetById(id);
        if (movie is null)
        {
            return null;
        }

        return new GetMovieByIdDTO
        {
            Title = movie.Title,
            Link = movie.Link
        };
    }

    public int CreateMovieExecution(CreateMovieDTO createMovieDTO)
    {
        var movie = new Domain.Entities.Movie.MovieData
        {
            Title = createMovieDTO.Title,
            Genre = createMovieDTO.Genre,
            Country = createMovieDTO.Country,
            ReleaseDate = createMovieDTO.ReleaseDate,
            Rating = createMovieDTO.Rating,
            Description = createMovieDTO.Description,
            Link = createMovieDTO.Link
        };

        _movieRepository.Add(movie);
        return movie.Id;
    }
}
