using System;
using System.Linq;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

static class MediaUrl
{
    internal static string Poster(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty :
        (value.StartsWith("http://") || value.StartsWith("https://")) ? value :
        $"/api/media/image/posters/{value}";

    internal static string Thumbnail(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty :
        (value.StartsWith("http://") || value.StartsWith("https://")) ? value :
        $"/api/media/image/thumbnails/{value}";

    internal static string Backdrop(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty :
        (value.StartsWith("http://") || value.StartsWith("https://")) ? value :
        $"/api/media/image/backdrops/{value}";
}

public partial class FrontendActions
{
    public HomePayloadDto GetHome()
    {
        var allMovies = _context.Movies
            .Include(m => m.Genres)
            .Where(m => !m.IsPrivate)
            .OrderByDescending(m => m.Score ?? 0)
            .ToList();

        var featuredMovie = allMovies.FirstOrDefault();

        var trending = allMovies.Take(4).Select(MapToContentCard).ToList();

        var currentYear = DateTime.Now.Year;
        var newReleases = allMovies
            .Where(m => (m.Year ?? 0) >= (currentYear - 1))
            .OrderByDescending(m => m.Year)
            .Take(3)
            .Select(MapToContentCard)
            .ToList();

        var topRated = allMovies.Take(3).Select(MapToContentCard).ToList();

        var featured = featuredMovie != null
            ? new FeaturedContentDto
            {
                Title = featuredMovie.Title ?? string.Empty,
                Type = featuredMovie.Type ?? featuredMovie.Category ?? string.Empty,
                Genre = string.Join(", ", (featuredMovie.Genres ?? []).Select(g => g.Name ?? string.Empty)),
                Year = featuredMovie.Year ?? 0,
                Score = featuredMovie.Score ?? 0,
                Views = featuredMovie.Views ?? string.Empty,
                Description = featuredMovie.Description ?? string.Empty,
                Poster = MediaUrl.Backdrop(featuredMovie.Backdrop) is { Length: >0 } b ? b : MediaUrl.Poster(featuredMovie.Poster)
            }
            : new FeaturedContentDto();

        return new HomePayloadDto
        {
            Featured = featured,
            TrendingRow = trending,
            NewReleases = newReleases,
            TopRated = topRated
        };
    }

    private static ContentCardDto MapToContentCard(Domain.Entities.Movie.MovieData m)
    {
        return new ContentCardDto
        {
            Id = m.Id.ToString(),
            Title = m.Title ?? string.Empty,
            Type = m.Type ?? m.Category ?? string.Empty,
            Genre = string.Join(", ", (m.Genres ?? []).Select(g => g.Name ?? string.Empty)),
            Year = m.Year ?? 0,
            Score = m.Score ?? 0,
            Poster = MediaUrl.Thumbnail(m.Thumbnail) is { Length: >0 } t ? t : MediaUrl.Poster(m.Poster)
        };
    }

    private static long ParseViews(string? views)
    {
        if (string.IsNullOrEmpty(views)) return 0;
        var trimmed = new string(views.Where(c => char.IsAsciiDigit(c)).ToArray());
        return long.TryParse(trimmed, out var n) ? n : 0;
    }

    public TrendingPayloadDto GetTrending(string? period = null, string? typeFilter = null)
    {
        var query = _context.Movies
            .Include(m => m.Genres)
            .Where(m => !m.IsPrivate);

        if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "all")
            query = query.Where(m => m.Type != null && m.Type.ToLower() == typeFilter.ToLower());

        var allItems = query.ToList();

        var sorted = period switch
        {
            "today" => allItems.OrderByDescending(m => ParseViews(m.Views)).ThenByDescending(m => m.Score ?? 0).ToList(),
            "month" => allItems.OrderByDescending(m => (m.Score ?? 0) * 0.7 + ParseViews(m.Views) * 0.3).ToList(),
            _ => allItems.OrderByDescending(m => m.Score ?? 0).ToList(),
        };

        var topItems = sorted.Take(20).ToList();

        var rng = Random.Shared;
        var data = topItems
            .Select((m, idx) => new TrendingItemDto
            {
                Id = m.Id.ToString(),
                Rank = idx + 1,
                Title = m.Title ?? string.Empty,
                Type = m.Type ?? m.Category ?? string.Empty,
                Genre = string.Join(", ", m.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>()),
                Year = m.Year ?? 0,
                Score = m.Score ?? 0,
                Views = m.Views ?? string.Empty,
                TrendPct = $"+{rng.Next(5, 99)}%",
                Description = m.Description ?? string.Empty,
                Thumb = MediaUrl.Thumbnail(m.Thumbnail) is { Length: >0 } t ? t : MediaUrl.Poster(m.Poster)
            })
            .ToList();

        return new TrendingPayloadDto
        {
            Periods = new List<UiOptionDto>
            {
                new() { Id = "today", Label = "Today" },
                new() { Id = "week", Label = "This Week" },
                new() { Id = "month", Label = "This Month" }
            },
            Filters = new List<UiOptionDto>
            {
                new() { Id = "all", Label = "All" },
                new() { Id = "movie", Label = "Movies" },
                new() { Id = "tv", Label = "Series" },
                new() { Id = "documentary", Label = "Documentaries" }
            },
            Data = data
        };
    }

    private static readonly Dictionary<string, string> GenreIcons = new()
    {
        ["Action"] = "bxs-joystick-alt",
        ["Comedy"] = "bx-smile",
        ["Drama"] = "bx-camera-movie",
        ["Horror"] = "bxs-ghost",
        ["Romance"] = "bxs-heart",
        ["Sci-Fi"] = "bx-planet",
        ["Thriller"] = "bxs-sad",
        ["Animation"] = "bxs-color-fill",
        ["Fantasy"] = "bxs-magic-wand",
        ["Adventure"] = "bxs-compass",
        ["Mystery"] = "bxs-search-alt-2",
        ["Crime"] = "bxs-badge-check",
        ["Documentary"] = "bxs-videos",
        ["Musical"] = "bxs-music",
        ["Family"] = "bxs-group",
    };

    private static readonly Dictionary<string, string[]> GenreTags = new()
    {
        ["Action"] = new[] { "New Releases", "Top Rated", "Blockbusters", "Action Thrillers" },
        ["Comedy"] = new[] { "Stand-up", "Rom-coms", "Dark Comedy", "Family Comedy" },
        ["Drama"] = new[] { "Award Winners", "Emotional", "Based on True Story", "Classics" },
        ["Horror"] = new[] { "Supernatural", "Psychological", "Slasher", "Paranormal" },
        ["Romance"] = new[] { "Love Stories", "Romantic Comedies", "Chick Flicks", "Period Romance" },
        ["Sci-Fi"] = new[] { "Space Exploration", "Time Travel", "Dystopian", "Alien Invasion" },
        ["Thriller"] = new[] { "Psychological", "Crime Thriller", "Mystery", "Suspense" },
        ["Animation"] = new[] { "CGI", "Hand-drawn", "Anime", "Stop Motion" },
        ["Fantasy"] = new[] { "Epic Fantasy", "Urban Fantasy", "Magical Realism", "Mythical" },
        ["Adventure"] = new[] { "Action Adventure", "Exploration", "Survival", "Treasure Hunt" },
        ["Mystery"] = new[] { "Whodunnit", "Noir", "Cozy Mystery", "Unsolved" },
        ["Crime"] = new[] { "Heist", "Detective", "Mafia", "True Crime" },
        ["Documentary"] = new[] { "Nature", "History", "Science", "Biography" },
        ["Musical"] = new[] { "Broadway", "Concert Films", "Dance", "Music Biopics" },
        ["Family"] = new[] { "Kids", "Family Adventure", "Animated", "Animal Tales" },
    };

    public ExplorePayloadDto GetExplore()
    {
        var categories = _context.Categories.ToList();
        var movies = _context.Movies
            .Include(m => m.Genres)
            .Where(m => !m.IsPrivate)
            .ToList();

        return new ExplorePayloadDto
        {
            Categories = categories.Select(c =>
            {
                var name = c.Name ?? "";
                return new ExploreCategoryDto
                {
                    Id = name.ToLower().Replace(" ", "-"),
                    Title = c.Title ?? name,
                    Icon = c.Icon ?? GenreIcons.GetValueOrDefault(name, "bx-category"),
                    Items = !string.IsNullOrEmpty(c.Items)
                        ? c.Items.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                        : GenreTags.GetValueOrDefault(name, Array.Empty<string>()).ToList()
                };
            }).ToList(),

            MovieDatabase = movies.Select(m => new ExploreMovieDto
            {
                Id = m.Id.ToString(),
                Title = m.Title ?? string.Empty,
                Year = m.Year ?? 0,
                Score = m.Score,
                Type = m.Type ?? m.Category ?? string.Empty,
                Genre = string.Join(", ", (m.Genres ?? []).Select(g => g.Name ?? string.Empty)),
                Poster = MediaUrl.Poster(m.Poster) is { Length: >0 } p ? p : MediaUrl.Thumbnail(m.Thumbnail)
            }).ToList(),

            CarouselRows = movies
                .Where(m => m.Genres != null && m.Genres.Any())
                .SelectMany(m => m.Genres!.Select(g => new { Movie = m, Genre = g }))
                .GroupBy(x => new { x.Genre.Id, Name = x.Genre.Name ?? "Other" })
                .Select(g => new CarouselRowDto
                {
                    Id = g.Key.Name.ToLower().Replace(" ", "-"),
                    Title = g.Key.Name,
                    Items = g.Select(x => new CarouselItemDto
                    {
                        Id = x.Movie.Id.ToString(),
                        Title = x.Movie.Title ?? string.Empty,
                        Meta = x.Movie.Type ?? x.Movie.Category ?? string.Empty,
                        Poster = MediaUrl.Poster(x.Movie.Poster) is { Length: >0 } p ? p : MediaUrl.Thumbnail(x.Movie.Thumbnail)
                    }).ToList()
                })
                .ToList()
        };
    }

    public ContentCardDto CreateContent(CreateContentDTO createDto)
    {
        string id = $"c-{new Random().Next(1000, 9999)}";
        string genreString = string.Join(", ", createDto.Genres ?? new List<string>());

        return new ContentCardDto
        {
            Id = id,
            Title = createDto.Title,
            Type = createDto.Type,
            Genre = genreString,
            Score = createDto.Score,
            Year = createDto.Year,
            Poster = "https://via.placeholder.com/130x190/1a1a2e/e0e0e0?text=NEW"
        };
    }
}
