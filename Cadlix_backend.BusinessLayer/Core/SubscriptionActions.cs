using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLayer.Core;

public class SubscriptionActions : ISubscriptionAction
{
    private readonly AppDbContext _context;
    private static readonly Dictionary<SubscriptionPlan, decimal> PlanPrices = new()
    {
        { SubscriptionPlan.Free, 9.99m },
        { SubscriptionPlan.Standard, 19.99m },
        { SubscriptionPlan.Premium, 29.99m }
    };

    public SubscriptionActions()
    {
        _context = new AppDbContext();
    }

    public SubscriptionDTO? GetActiveSubscription(int userId)
    {
        var subscription = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == userId);
        if (subscription is null || !IsActive(subscription))
        {
            return null;
        }

        return MapToDto(subscription);
    }

    public SubscriptionDTO? CreateSubscription(CreateSubscriptionDTO createDto)
    {
        var existing = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == createDto.UserId);

        if (existing is not null && IsActive(existing))
        {
            return null;
        }

        if (existing is not null)
        {
            existing.Name = createDto.Plan.ToString();
            existing.Price = PlanPrices[createDto.Plan];
            existing.DurationInMonths = 1;
            existing.Features = string.Empty;
            existing.CreatedAt = DateTime.UtcNow;
            existing.IsCanceled = false;
            _context.SaveChanges();
            return MapToDto(existing);
        }

        var created = new Cadlix_backend.Domain.Entities.Subscription.SubscriptionData
        {
            UserId = createDto.UserId,
            Name = createDto.Plan.ToString(),
            Price = PlanPrices[createDto.Plan],
            DurationInMonths = 1,
            Features = string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsCanceled = false
        };

        _context.Subscriptions.Add(created);
        _context.SaveChanges();
        return MapToDto(created);
    }

    public SubscriptionDTO? UpgradePlan(int userId, SubscriptionPlan newPlan)
    {
        var subscription = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == userId);
        if (subscription is null || !IsActive(subscription))
        {
            return null;
        }

        subscription.Name = newPlan.ToString();
        subscription.Price = PlanPrices[newPlan];
        _context.SaveChanges();
        return MapToDto(subscription);
    }

    public bool CancelSubscription(int userId)
    {
        var subscription = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == userId);
        if (subscription is null)
        {
            return false;
        }

        subscription.IsCanceled = true;
        _context.SaveChanges();
        return true;
    }

    public bool HasActiveSubscription(int userId)
    {
        var subscription = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == userId);
        return subscription is not null && IsActive(subscription);
    }

    public bool CanAccessContent(int userId, SubscriptionPlan requiredPlan)
    {
        var subscription = _context.Subscriptions.FirstOrDefault(entity => entity.UserId == userId);

        if (subscription is null || !IsActive(subscription))
        {
            return false;
        }

        var currentPlan = ParsePlan(subscription.Name);
        return currentPlan >= requiredPlan;
    }

    public void RenewExpiredSubscriptions()
    {
        var now = DateTime.UtcNow;
        var expired = _context.Subscriptions
            .Where(entity => !entity.IsCanceled && entity.CreatedAt.AddMonths(entity.DurationInMonths) <= now)
            .ToList();

        foreach (var subscription in expired)
        {
            subscription.CreatedAt = now;
        }

        if (expired.Count > 0)
        {
            _context.SaveChanges();
        }
    }

    private static bool IsActive(Cadlix_backend.Domain.Entities.Subscription.SubscriptionData subscription)
    {
        return !subscription.IsCanceled
            && subscription.CreatedAt.AddMonths(subscription.DurationInMonths) > DateTime.UtcNow;
    }

    private static SubscriptionPlan ParsePlan(string? planName)
    {
        return Enum.TryParse<SubscriptionPlan>(planName, true, out var parsedPlan)
            ? parsedPlan
            : SubscriptionPlan.Free;
    }

    private static SubscriptionDTO MapToDto(Cadlix_backend.Domain.Entities.Subscription.SubscriptionData subscription)
    {
        var endDate = subscription.CreatedAt.AddMonths(subscription.DurationInMonths);
        var isActive = !subscription.IsCanceled && endDate > DateTime.UtcNow;

        return new SubscriptionDTO
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            Plan = ParsePlan(subscription.Name),
            Price = subscription.Price,
            StartDate = subscription.CreatedAt,
            EndDate = endDate,
            IsActive = isActive,
            DaysRemaining = isActive
                ? Math.Max(0, (int)(endDate - DateTime.UtcNow).TotalDays)
                : 0
        };
    }
}
