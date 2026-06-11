using AutoMapper;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.History;
using Cadlix_backend.Domain.Entities.History;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.BusinessLayer.Core;

public class HistoryActions : IHistoryAction
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public HistoryActions(IMapper mapper)
    {
        _context = new AppDbContext();
        _mapper = mapper;
    }

    public IEnumerable<HistoryDTO> GetAllHistory()
    {
        var histories = _context.Histories.ToList();
        return _mapper.Map<List<HistoryDTO>>(histories);
    }

    private Dictionary<string, int> ComputeSeriesProgress(IEnumerable<HistoryData> userHistory)
    {
        var seriesNames = userHistory
            .Select(h => h.Series)
            .Where(s => !string.IsNullOrEmpty(s) && s != "-")
            .Distinct()
            .ToList();

        var cache = new Dictionary<string, int>();
        if (seriesNames.Count == 0) return cache;

        var seriesMovies = _context.Movies
            .Where(m => m.Series != null && seriesNames.Contains(m.Series) && m.Type == "Series")
            .ToList();

        foreach (var group in seriesMovies.GroupBy(m => m.Series!))
        {
            var totalDuration = group.Sum(e => e.DurationSeconds).GetValueOrDefault();
            if (totalDuration <= 0) continue;

            var userForSeries = userHistory.Where(h => group.Any(e => e.Id == h.MovieId)).ToList();
            var watchedDuration = userForSeries.Sum(h =>
            {
                var ep = group.FirstOrDefault(e => e.Id == h.MovieId);
                return ep != null ? (int)(h.ProgressPercentage / 100.0 * (ep.DurationSeconds ?? 0)) : 0;
            });

            cache[group.Key] = Math.Min(100, (int)((double)watchedDuration / Math.Max(totalDuration, 1) * 100));
        }

        return cache;
    }

    public IEnumerable<HistoryDTO> GetHistoryByUserId(int userId)
    {
        var histories = _context.Histories
            .Include(h => h.Movie)
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.WatchedAt)
            .ToList();

        var dtos = _mapper.Map<List<HistoryDTO>>(histories);
        var seriesProgress = ComputeSeriesProgress(histories);

        foreach (var dto in dtos)
        {
            var history = histories.FirstOrDefault(h => h.Id == dto.Id);
            if (history?.Movie != null)
                dto.Poster = MediaUrl.Poster(history.Movie.Poster) is { Length: >0 } p ? p : MediaUrl.Thumbnail(history.Movie.Thumbnail);

            if (!string.IsNullOrEmpty(history?.Series) && history.Series != "-")
            {
                var pct = seriesProgress.GetValueOrDefault(history.Series, dto.ProgressPercentage);
                dto.ProgressPercentage = pct;
                dto.Progress = $"{pct}%";
            }
        }
        return dtos;
    }

    public HistoryDTO? GetHistoryById(int id)
    {
        var history = _context.Histories
            .Include(h => h.Movie)
            .FirstOrDefault(h => h.Id == id);
        if (history == null) return null;
        var dto = _mapper.Map<HistoryDTO>(history);
        if (history.Movie != null)
            dto.Poster = MediaUrl.Poster(history.Movie.Poster) is { Length: >0 } p ? p : MediaUrl.Thumbnail(history.Movie.Thumbnail);

        if (!string.IsNullOrEmpty(history.Series) && history.Series != "-")
        {
            var allUserHistory = _context.Histories.Where(h => h.UserId == history.UserId).ToList();
            var seriesProgress = ComputeSeriesProgress(allUserHistory);
            var pct = seriesProgress.GetValueOrDefault(history.Series, dto.ProgressPercentage);
            dto.ProgressPercentage = pct;
            dto.Progress = $"{pct}%";
        }

        return dto;
    }

    public HistoryDTO CreateHistory(CreateHistoryDTO dto)
    {
        var response = _context.Histories.SingleOrDefault(h => h.UserId == dto.UserId && h.MovieId == dto.MovieId);
        if (response != null)
        {
            response.WatchedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return _mapper.Map<HistoryDTO>(response);
        }

        var history = _mapper.Map<HistoryData>(dto);
        history.WatchedAt = DateTime.UtcNow;
        _context.Histories.Add(history);
        _context.SaveChanges();

        return _mapper.Map<HistoryDTO>(history);
    }

    public HistoryDTO? UpdateHistory(int id, UpdateHistoryDTO dto)
    {
        var history = _context.Histories.FirstOrDefault(h => h.Id == id);
        if (history == null)
            return null;

        _mapper.Map(dto, history);
        _context.SaveChanges();

        return _mapper.Map<HistoryDTO>(history);
    }

    public bool DeleteHistory(int id)
    {
        var history = _context.Histories.FirstOrDefault(h => h.Id == id);
        if (history == null)
            return false;

        _context.Histories.Remove(history);
        _context.SaveChanges();
        return true;
    }

    public void DeleteUserHistory(int userId)
    {
        var histories = _context.Histories.Where(h => h.UserId == userId).ToList();
        _context.Histories.RemoveRange(histories);
        _context.SaveChanges();
    }
}
