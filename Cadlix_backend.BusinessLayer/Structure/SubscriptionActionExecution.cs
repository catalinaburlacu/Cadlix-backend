using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Structure;

public class SubscriptionActionExecution : SubscriptionActions, ISubscriptionAction
{
    public SubscriptionDTO? GetActiveSubscription(int userId)
    {
        return GetActiveSubscription(userId);
    }
    public SubscriptionDTO? CreateSubscription(CreateSubscriptionDTO createDto)
    {
        return CreateSubscription(createDto);
    }
    public SubscriptionDTO? UpgradePlan(int userId, SubscriptionPlan newPlan)
    {
        return UpgradePlan(userId, newPlan);
    }
    public void CancelSubscription(int userId)
    {
        CancelSubscription(userId);
    }
    public bool HasActiveSubscription(int userId)
    {
        return HasActiveSubscription(userId);
    }
    public bool CanAccessContent(int userId, SubscriptionPlan requiredPlan)
    {
        return CanAccessContent(userId, requiredPlan);
    }
    public void RenewExpiredSubscriptions()
    {
        RenewExpiredSubscriptions();
    }
}
