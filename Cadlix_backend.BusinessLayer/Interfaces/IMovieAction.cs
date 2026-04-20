using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IMovieAction
{
    List<GetMovieDetailDTO> GetAllMovies();
    GetMovieByIdDTO? GetMovieById(int id);
    int CreateMovie(CreateMovieDTO createMovieDTO);
}
