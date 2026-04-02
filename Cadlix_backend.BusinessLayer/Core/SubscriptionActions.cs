using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Services;

public class SubscriptionActions
{
    private readonly ISubscriptionRepository _repo;

    public SubscriptionActions()
    {
        _repo = new SubscriptionRepository( new AppDbContext());
    }

    public async Task<SubscriptionDTO> GetActiveSubscriptionAsync(int userId)
    {
        return await _repo.GetActiveSubscriptionAsync(userId);
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(CreateSubscriptionDTO createDto)
    {
        return await _repo.CreateSubscriptionAsync(createDto);
    }

    public async Task<SubscriptionDTO> UpgradePlanAsync(int userId, SubscriptionPlan newPlan)
    {
        return await _repo.UpgradePlanAsync(userId, newPlan);
    }

    public async Task CancelSubscriptionAsync(int userId)
    {
        await _repo.CancelSubscriptionAsync(userId);
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        return await _repo.HasActiveSubscriptionAsync(userId);
    }

    public async Task<bool> CanAccessContentAsync(int userId, SubscriptionPlan requiredPlan)
    {
        return await _repo.CanAccessContentAsync(userId, requiredPlan);
    }

    public async Task RenewExpiredSubscriptionsAsync()
    {
        await _repo.RenewExpiredSubscriptionsAsync();
    }
}
