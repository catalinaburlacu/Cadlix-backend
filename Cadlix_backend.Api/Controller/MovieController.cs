using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cadlix_backend.BusinessLogic.Services;
using Cadlix_backend.Domain.DTOs.Movie;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieDTO createMovieDTO)
        {
            await _movieService.CreateMovieAsync(createMovieDTO);
            return CreatedAtAction(nameof(GetMovieById), new { id = createMovieDTO.Id }, createMovieDTO);
        }
    }
}
