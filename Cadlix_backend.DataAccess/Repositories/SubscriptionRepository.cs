using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Enum;
using Cadlix_backend.Domain.Entities.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;
    private static readonly Dictionary<SubscriptionPlan, decimal> PlanPrices = new()
    {
        { SubscriptionPlan.Free, 9.99m },
        { SubscriptionPlan.Standard, 19.99m },
        { SubscriptionPlan.Premium, 29.99m }
    };

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionData>> GetAllAsync()
    {
        return await _context.Subscriptions.ToListAsync();
    }

    public async Task<SubscriptionData?> GetByIdAsync(int id)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<SubscriptionData> AddAsync(SubscriptionData entity)
    {
        await _context.Subscriptions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<SubscriptionData?> UpdateAsync(SubscriptionData entity)
    {
        var existing = await GetByIdAsync(entity.Id);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Subscriptions.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SubscriptionDTO> GetActiveSubscriptionAsync(int userId)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.UserId == userId);

        if (subscription is null || !IsActive(subscription))
        {
            throw new InvalidOperationException("User does not have an active subscription.");
        }

        return MapToDto(subscription);
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto)
    {
        var existing = await _context.Subscriptions
            .FirstOrDefaultAsync(entity => entity.UserId == dto.UserId);

        if (existing is not null && IsActive(existing))
        {
            throw new InvalidOperationException("User already has an active subscription.");
        }

        if (existing is not null)
        {
            existing.Name = dto.Plan.ToString();
            existing.Price = PlanPrices[dto.Plan];
            existing.DurationInMonths = 1;
            existing.Features = string.Empty;
            existing.CreatedAt = DateTime.UtcNow;
            existing.IsCanceled = false;
            await _context.SaveChangesAsync();
            return MapToDto(existing);
        }

        var created = new SubscriptionData
        {
            UserId = dto.UserId,
            Name = dto.Plan.ToString(),
            Price = PlanPrices[dto.Plan],
            DurationInMonths = 1,
            Features = string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsCanceled = false
        };

        await _context.Subscriptions.AddAsync(created);
        await _context.SaveChangesAsync();

        return MapToDto(created);
    }

    public async Task<SubscriptionDTO> UpgradePlanAsync(int userId, SubscriptionPlan newPlan)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(entity => entity.UserId == userId);

        if (subscription is null || !IsActive(subscription))
        {
            throw new InvalidOperationException("User does not have an active subscription.");
        }

        subscription.Name = newPlan.ToString();
        subscription.Price = PlanPrices[newPlan];

        await _context.SaveChangesAsync();
        return MapToDto(subscription);
    }

    public async Task CancelSubscriptionAsync(int userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(entity => entity.UserId == userId);

        if (subscription is null)
        {
            throw new InvalidOperationException("Subscription was not found.");
        }

        subscription.IsCanceled = true;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.UserId == userId);

        return subscription is not null && IsActive(subscription);
    }

    public async Task<bool> CanAccessContentAsync(int userId, SubscriptionPlan requiredPlan)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.UserId == userId);

        if (subscription is null || !IsActive(subscription))
        {
            return false;
        }

        var currentPlan = ParsePlan(subscription.Name);
        return currentPlan >= requiredPlan;
    }

    public async Task RenewExpiredSubscriptionsAsync()
    {
        var now = DateTime.UtcNow;

        var expired = await _context.Subscriptions
            .Where(entity => !entity.IsCanceled && entity.CreatedAt.AddMonths(entity.DurationInMonths) <= now)
            .ToListAsync();

        foreach (var subscription in expired)
        {
            subscription.CreatedAt = now;
        }

        if (expired.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }

    private static bool IsActive(SubscriptionData subscription)
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

    private static SubscriptionDTO MapToDto(SubscriptionData subscription)
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
