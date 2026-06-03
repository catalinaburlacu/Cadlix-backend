using Cadlix_backend.Domain.DTOs.History;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IHistoryAction
{
    IEnumerable<HistoryDTO> GetAllHistory();
    IEnumerable<HistoryDTO> GetHistoryByUserId(int userId);
    HistoryDTO? GetHistoryById(int id);
    HistoryDTO CreateHistory(CreateHistoryDTO dto);
    HistoryDTO? UpdateHistory(int id, UpdateHistoryDTO dto);
    bool DeleteHistory(int id);
    void DeleteUserHistory(int userId);
}
