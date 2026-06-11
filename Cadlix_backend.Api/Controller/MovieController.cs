using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cadlix_backend.Domain.DTOs.Movie;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer;
using Cadlix_backend.Api.Utilities;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieAction _movieService;
        private readonly FileUploadHandler _fileUploadHandler;
        private readonly ILogger<MovieController> _logger;

        public MovieController(ILogger<MovieController> logger)
        {
            _movieService = new BusinessLogic().MovieAction();
            _fileUploadHandler = new FileUploadHandler(logger);
            _logger = logger;
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

        /// <summary>
        /// Upload a movie with video file
        /// </summary>
        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMovie([FromForm] UploadMovieFormDto form)
        {
            try
            {
                if (form == null)
                    return BadRequest("Movie data is required");

                if (form.VideoFile == null || form.VideoFile.Length == 0)
                    return BadRequest("Video file is required");

                var createMovieDTO = new CreateMovieDTO
                {
                    Title = form.Title,
                    Description = form.Description,
                    Genres = form.Genres,
                    Country = form.Country,
                    Year = form.Year
                };

                var movieId = _movieService.CreateMovie(createMovieDTO);

                var (uploadSuccess, fileName, errorMessage) = await _fileUploadHandler.UploadVideoAsync(form.VideoFile, movieId);

                if (!uploadSuccess)
                {
                    _logger.LogWarning($"Video upload failed for movie {movieId}: {errorMessage}");
                    return BadRequest(new MovieUploadResponseDTO
                    {
                        MovieId = movieId,
                        Success = false,
                        Message = errorMessage
                    });
                }

                var updateDto = new Cadlix_backend.Domain.DTOs.Content.UpdateContentDTO
                {
                    VideoSource = fileName,
                    Description = form.Description,
                    Director = form.Director,
                    Cast = form.Cast,
                    Country = form.Country,
                    Category = form.Category,
                    Duration = form.Duration,
                    DurationSeconds = form.DurationSeconds,
                };

                var contentService = new BusinessLogic().Content();
                contentService.UpdateContent(movieId, updateDto);

                return Ok(new MovieUploadResponseDTO
                {
                    MovieId = movieId,
                    Title = form.Title,
                    VideoFileName = fileName,
                    Success = true,
                    Message = "Movie uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading movie: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new MovieUploadResponseDTO
                    {
                        Success = false,
                        Message = "Error uploading movie"
                    });
            }
        }

        /// <summary>
        /// Get streaming URL for a movie
        /// </summary>
        [HttpGet("{id}/stream")]
        [AllowAnonymous]
        public IActionResult GetStreamUrl(int id)
        {
            try
            {
                var movie = _movieService.GetMovieById(id);
                if (movie is null)
                    return NotFound("Movie not found");

                var streamUrl = !string.IsNullOrEmpty(movie.Link)
                    ? $"/api/streaming/{movie.Link}"
                    : null;

                return Ok(new { streamUrl, movieId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting stream URL: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting stream URL");
            }
        }
    }
}
