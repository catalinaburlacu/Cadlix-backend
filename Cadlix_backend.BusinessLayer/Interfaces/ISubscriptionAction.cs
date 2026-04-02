using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface ISubscriptionAction
{
    SubscriptionDTO GetActiveSubscription(int userId);
    SubscriptionDTO CreateSubscription(CreateSubscriptionDTO createDto);
    SubscriptionDTO UpgradePlan(int userId, SubscriptionPlan newPlan);
    void CancelSubscription(int userId);
    bool HasActiveSubscription(int userId);
    bool CanAccessContent(int userId, SubscriptionPlan requiredPlan);
    void RenewExpiredSubscriptions();
}
