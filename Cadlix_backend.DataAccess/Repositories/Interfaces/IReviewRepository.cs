namespace Cadlix_backend.DataAccess.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<double> GetAverageRatingAsync(int userId);
    Task<int> GetReviewCountAsync(int userId);
    Task<int> GetTotalLikesReceivedAsync(int userId);
}
