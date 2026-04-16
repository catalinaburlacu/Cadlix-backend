using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cadlix_backend.Domain.DTOs.Movie;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieAction _movieService;

        public MovieController()
        {
            _movieService = new BusinessLogic().MovieAction();
        }

        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _movieService.GetAllMovies();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        public IActionResult CreateMovie(CreateMovieDTO createMovieDTO)
        {
            var id = _movieService.CreateMovie(createMovieDTO);
            createMovieDTO.Id = id;
            return CreatedAtAction(nameof(GetMovieById), new { id }, createMovieDTO);
        }
    }
}
