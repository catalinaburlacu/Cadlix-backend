using System;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.BusinessLayer.Services;

public interface IMovieService
{
    Task<List<GetMovieDetailDTO>> GetAllMoviesAsync();
    Task<GetMovieByIdDTO?> GetMovieByIdAsync(int id);
    Task CreateMovieAsync(CreateMovieDTO createMovieDTO);
}
