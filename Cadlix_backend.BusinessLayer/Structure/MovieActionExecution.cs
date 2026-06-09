using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Structure;

public class MovieActionExecution : IMovieAction
{
    private readonly MovieAction _core;

    public MovieActionExecution()
    {
        _core = new MovieAction();
    }

    public List<GetMovieDetailDTO> GetAllMovies()
    {
        return _core.GetAllMoviesExecution();
    }

    public GetMovieByIdDTO? GetMovieById(int id)
    {
        return _core.GetMovieByIdExecution(id);
    }

    public int CreateMovie(CreateMovieDTO createMovieDTO)
    {
        return _core.CreateMovieExecution(createMovieDTO);
    }
}
