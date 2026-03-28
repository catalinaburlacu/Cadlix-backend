using Cadlix_backend.Domain.Entities.Leaderboard;
using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.DataAccess.Repositories.Interfaces;

public interface ILeaderboardRepository : IRepository<LeaderboardData>
{
    Task<IEnumerable<LeaderboardEntryDto>> GetTopUsersAsync(int count = 100);
    Task<LeaderboardEntryDto> GetUserRankAsync(int userId);
    Task<double> CalculateScoreAsync(int userId);
}
