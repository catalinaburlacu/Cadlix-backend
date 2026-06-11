using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Structure;

public class SubscriptionActionExecution : SubscriptionActions, ISubscriptionAction
{
    public new SubscriptionDTO? GetActiveSubscription(int userId)
    {
        return base.GetActiveSubscription(userId);
    }
    public new SubscriptionDTO? CreateSubscription(CreateSubscriptionDTO createDto)
    {
        return base.CreateSubscription(createDto);
    }
    public new SubscriptionDTO? UpgradePlan(int userId, SubscriptionPlan newPlan)
    {
        return base.UpgradePlan(userId, newPlan);
    }
    public new void CancelSubscription(int userId)
    {
        base.CancelSubscription(userId);
    }
    public new bool HasActiveSubscription(int userId)
    {
        return base.HasActiveSubscription(userId);
    }
    public new bool CanAccessContent(int userId, SubscriptionPlan requiredPlan)
    {
        return base.CanAccessContent(userId, requiredPlan);
    }
    public new void RenewExpiredSubscriptions()
    {
        base.RenewExpiredSubscriptions();
    }
}
