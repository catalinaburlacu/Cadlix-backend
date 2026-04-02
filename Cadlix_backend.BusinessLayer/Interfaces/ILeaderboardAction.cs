using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface ILeaderboardAction
{
    IEnumerable<LeaderboardEntryDto> GetTopUsers(int count = 100);
    LeaderboardEntryDto? GetUserRank(int userId);
    double CalculateScore(int userId);
}
