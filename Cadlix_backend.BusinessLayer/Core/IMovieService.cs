using System;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Core;

public interface IMovieService
{
    List<GetMovieDetailDTO> GetAllMovies();
    GetMovieByIdDTO? GetMovieById(int id);
    void CreateMovie(CreateMovieDTO createMovieDTO);
}
