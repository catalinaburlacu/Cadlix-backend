namespace Cadlix_backend.DataAccess.Repositories.Interfaces;

public interface IWatchHistoryRepository
{
    Task<double> GetTotalWatchHoursAsync(int userId);
    Task<int> GetMoviesWatchedCountAsync(int userId);
    Task<int> GetEpisodesWatchedCountAsync(int userId);
}
