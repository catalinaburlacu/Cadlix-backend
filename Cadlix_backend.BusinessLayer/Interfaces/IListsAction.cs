using Cadlix_backend.Domain.DTOs.Lists;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IListsAction
{
    IEnumerable<ListDTO> GetAllLists();
    IEnumerable<ListDTO> GetListsByUserId(int userId);
    IEnumerable<ListDTO> GetListsByStatus(int userId, string status);
    ListDTO? GetListById(int id);
    ListDTO CreateList(CreateListDTO dto);
    ListDTO? UpdateList(int id, UpdateListDTO dto);
    bool DeleteList(int id);
    void DeleteUserLists(int userId);
    bool UpdateFilmStatus(int id, string status);
}
