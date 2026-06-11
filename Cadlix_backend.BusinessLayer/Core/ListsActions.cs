using AutoMapper;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs.Lists;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;

namespace Cadlix_backend.BusinessLayer.Core;

public class ListsActions : IListsAction
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ListsActions(IMapper mapper)
    {
        _context = new AppDbContext();
        _mapper = mapper;
    }

    public IEnumerable<ListDTO> GetAllLists()
    {
        var lists = _context.Lists.ToList();
        return _mapper.Map<List<ListDTO>>(lists);
    }

    public IEnumerable<ListDTO> GetListsByUserId(int userId)
    {
        var lists = _context.Lists
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.AddedAt)
            .ToList();
        return _mapper.Map<List<ListDTO>>(lists);
    }

    public IEnumerable<ListDTO> GetListsByStatus(int userId, string status)
    {
        var lists = _context.Lists
            .Where(l => l.UserId == userId && l.FilmStatus == status)
            .OrderByDescending(l => l.AddedAt)
            .ToList();
        return _mapper.Map<List<ListDTO>>(lists);
    }

    public ListDTO? GetListById(int id)
    {
        var list = _context.Lists.FirstOrDefault(l => l.Id == id);
        return list == null ? null : _mapper.Map<ListDTO>(list);
    }

    public ListDTO CreateList(CreateListDTO dto)
    {
        var existing = _context.Lists
            .FirstOrDefault(l => l.UserId == dto.UserId && l.FilmId == dto.FilmId);

        if (existing != null)
        {
            _mapper.Map(dto, existing);
            existing.AddedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return _mapper.Map<ListDTO>(existing);
        }

        var list = _mapper.Map<ListsData>(dto);
        list.AddedAt = DateTime.UtcNow;
        _context.Lists.Add(list);
        _context.SaveChanges();

        return _mapper.Map<ListDTO>(list);
    }

    public ListDTO? UpdateList(int id, UpdateListDTO dto)
    {
        var list = _context.Lists.FirstOrDefault(l => l.Id == id);
        if (list == null)
            return null;

        _mapper.Map(dto, list);
        _context.SaveChanges();

        return _mapper.Map<ListDTO>(list);
    }

    public bool DeleteList(int id)
    {
        var list = _context.Lists.FirstOrDefault(l => l.Id == id);
        if (list == null)
            return false;

        _context.Lists.Remove(list);
        _context.SaveChanges();
        return true;
    }

    public void DeleteUserLists(int userId)
    {
        var lists = _context.Lists.Where(l => l.UserId == userId).ToList();
        _context.Lists.RemoveRange(lists);
        _context.SaveChanges();
    }

    public bool UpdateFilmStatus(int id, string status)
    {
        var list = _context.Lists.FirstOrDefault(l => l.Id == id);
        if (list == null)
            return false;

        list.FilmStatus = status;
        _context.SaveChanges();
        return true;
    }
}
