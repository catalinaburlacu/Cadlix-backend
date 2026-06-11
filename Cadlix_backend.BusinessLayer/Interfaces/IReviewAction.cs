using Cadlix_backend.Domain.DTOs.Review;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IReviewAction
{
    ReviewDTO? GetById(int id);
    List<ReviewDTO> GetByMovie(int movieId, int? currentUserId = null);
    List<ReviewDTO> GetByUser(int userId, int? currentUserId = null);
    ReviewDTO Create(int userId, CreateReviewDTO dto);
    ReviewDTO? Update(int id, int userId, CreateReviewDTO dto);
    bool Delete(int id, int userId);
    bool ToggleLike(int reviewId, int userId);
}
