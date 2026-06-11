using AutoMapper;
using Cadlix_backend.BusinessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Content;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.Movie;

namespace Cadlix_backend.BusinessLayer.Core;

public class ContentActions : IContentAction
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ContentActions(IMapper mapper)
    {
        _context = new AppDbContext();
        _mapper = mapper;
    }

    public IEnumerable<ContentDTO> GetAllContent()
    {
        var movies = _context.Movies
            .Include(m => m.Genres)
            .ToList();
        return _mapper.Map<List<ContentDTO>>(movies);
    }

    public IEnumerable<ContentDTO> GetContentByType(string type)
    {
        var movies = _context.Movies
            .Include(m => m.Genres)
            .Where(m => m.Type == type && !m.IsPrivate)
            .ToList();
        return _mapper.Map<List<ContentDTO>>(movies);
    }

    public ContentDTO? GetContentById(int id)
    {
        var movie = _context.Movies
            .Include(m => m.Genres)
            .FirstOrDefault(m => m.Id == id);
        return movie == null ? null : _mapper.Map<ContentDTO>(movie);
    }

    public ContentDTO CreateContent(CreateContentDTO dto)
    {
        var movie = new MovieData
        {
            Title = dto.Title,
            Type = dto.Type,
            Year = dto.Year,
            Score = (int?)dto.Score,
        };

        if (dto.Genres != null && dto.Genres.Count > 0)
        {
            var existing = _context.Categories
                .Where(c => c.Name != null && dto.Genres != null && dto.Genres.Contains(c.Name))
                .ToList();

            var newNames = dto.Genres
                .Where(name => !existing.Any(c => c.Name == name))
                .Select(name => new CategoryData { Name = name })
                .ToList();

            movie.Genres = existing.Concat(newNames).ToList();
        }

        _context.Movies.Add(movie);
        _context.SaveChanges();

        return _mapper.Map<ContentDTO>(movie);
    }

    public ContentDTO? UpdateContent(int id, UpdateContentDTO dto)
    {
        var movie = _context.Movies
            .Include(m => m.Genres)
            .FirstOrDefault(m => m.Id == id);
        if (movie == null)
            return null;

        _mapper.Map(dto, movie);

        if (dto.Genres != null)
        {
            var existing = _context.Categories
                .Where(c => c.Name != null && dto.Genres != null && dto.Genres.Contains(c.Name))
                .ToList();

            var newNames = dto.Genres
                .Where(name => !existing.Any(c => c.Name == name))
                .Select(name => new CategoryData { Name = name })
                .ToList();

            movie.Genres = existing.Concat(newNames).ToList();
        }

        _context.SaveChanges();

        return _mapper.Map<ContentDTO>(movie);
    }

    public bool DeleteContent(int id)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
            return false;

        _context.Movies.Remove(movie);
        _context.SaveChanges();
        return true;
    }

    public IEnumerable<ContentDTO> SearchContent(string query)
    {
        var lowerQuery = query.ToLower();

        var movies = _context.Movies
            .Include(m => m.Genres)
            .Where(m => !m.IsPrivate)
            .ToList();

        movies = movies
            .Where(m =>
                (m.Title != null && m.Title.ToLower().Contains(lowerQuery)) ||
                (m.Description != null && m.Description.ToLower().Contains(lowerQuery)) ||
                (m.Director != null && m.Director.ToLower().Contains(lowerQuery)) ||
                (m.Genres != null && m.Genres.Any(g => g.Name != null && g.Name.ToLower().Contains(lowerQuery))) ||
                (m.Country != null && m.Country.Any(c => c != null && c.ToLower().Contains(lowerQuery))) ||
                (m.Cast != null && m.Cast.Any(c => c != null && c.ToLower().Contains(lowerQuery))))
            .OrderBy(m => m.Title)
            .Take(20)
            .ToList();

        return _mapper.Map<List<ContentDTO>>(movies);
    }

    public IEnumerable<ContentDTO> GetSeriesEpisodes(string seriesName)
    {
        var movies = _context.Movies
            .Include(m => m.Genres)
            .Where(m => m.Series != null && m.Series == seriesName && !m.IsPrivate)
            .OrderBy(m => m.Episode)
            .ToList();
        return _mapper.Map<List<ContentDTO>>(movies);
    }
}
