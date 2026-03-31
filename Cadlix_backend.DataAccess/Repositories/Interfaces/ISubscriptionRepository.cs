using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;
using Cadlix_backend.Domain.Entities.Subscription;

namespace Cadlix_backend.DataAccess.Repositories.Interfaces;

public interface ISubscriptionRepository : IRepository<SubscriptionData>
{
    Task<SubscriptionDTO> GetActiveSubscriptionAsync(int userId);
    Task<SubscriptionDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto);
    Task<SubscriptionDTO> UpgradePlanAsync(int userId, SubscriptionPlan newPlan);
    Task CancelSubscriptionAsync(int userId);
    Task<bool> HasActiveSubscriptionAsync(int userId);
    Task<bool> CanAccessContentAsync(int userId, SubscriptionPlan requiredPlan);
    Task RenewExpiredSubscriptionsAsync();
}
