using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Services;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Structure;

public class SubscriptionActionExecution : SubscriptionActions, ISubscriptionAction
{
    public SubscriptionDTO GetActiveSubscription(int userId)
    {
        return GetActiveSubscriptionAsync(userId).GetAwaiter().GetResult();
    }
    public SubscriptionDTO CreateSubscription(CreateSubscriptionDTO createDto)
    {
        return CreateSubscriptionAsync(createDto).GetAwaiter().GetResult();
    }
    public SubscriptionDTO UpgradePlan(int userId, SubscriptionPlan newPlan)
    {
        return UpgradePlanAsync(userId, newPlan).GetAwaiter().GetResult();
    }
    public void CancelSubscription(int userId)
    {
        CancelSubscriptionAsync(userId).GetAwaiter().GetResult();
    }
    public bool HasActiveSubscription(int userId)
    {
        return HasActiveSubscriptionAsync(userId).GetAwaiter().GetResult();
    }
    public bool CanAccessContent(int userId, SubscriptionPlan requiredPlan)
    {
        return CanAccessContentAsync(userId, requiredPlan).GetAwaiter().GetResult();
    }
    public void RenewExpiredSubscriptions()
    {
        RenewExpiredSubscriptionsAsync().GetAwaiter().GetResult();
    }
}
