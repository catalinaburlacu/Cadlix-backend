using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Structure;

public class MovieActionExecution: MovieAction, IMovieAction
{
    public MovieActionExecution(){}

    public List<GetMovieDetailDTO> GetAllMovies()
    {
        return GetAllMoviesExecution();
    }

    public GetMovieByIdDTO? GetMovieById(int id)
    {
        return GetMovieByIdExecution(id);
    }

    public int CreateMovie(CreateMovieDTO movie)
    {
        return CreateMovieExecution(movie);
    }
}
