using System;
using Cadlix_backend.BusinessLogic.DTOs;
using Cadlix_backend.BusinessLogic.Enum;

namespace Cadlix_backend.BusinessLogic.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDTO> GetActiveSubscriptionAsync(int userId);
    Task<SubscriptionDTO> CreateSubscriptionAsync(CreateSubscriptionDTO createDto);
    Task<SubscriptionDTO> UpdatePlanAsync(int subscriptionId, SubscriptionPlan newPlan);
    Task CancelSubscriptionAsync(int subscriptionId);
    Task<bool> HasActiveSubscriptionAsync(int userId);
    Task<bool> CanAccessContentAsync(int userId);
    Task RenewSubscriptionAsync(int subscriptionId);
}
