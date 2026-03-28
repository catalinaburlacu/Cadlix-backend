using System;
using AutoMapper;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.Entities.Subscription;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.BusinessLogic.Services;

public class SubscriptionService
{
    private readonly ISubscriptionRepository _repo;
    private readonly IMapper _mapper;

    private static readonly Dictionary<SubscriptionPlan, decimal> PlanPrices = new()
    {
        { SubscriptionPlan.Free, 9.99m },
        { SubscriptionPlan.Standard, 19.99m },
        { SubscriptionPlan.Premium, 29.99m }
    };

    public SubscriptionService(ISubscriptionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<SubscriptionDTO> GetActiveSubscriptionAsync(int userId)
    {
        var subscription = await _repo.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User does not have an active subscription.");

        return MapToDto(subscription, userId, ParsePlan(subscription.Name));
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(CreateSubscriptionDTO createDto)
    {
        var existing = await _repo.GetByIdAsync(createDto.UserId);
        if (existing != null)
        {
            throw new InvalidOperationException("User already has a subscription.");
        }

        var subscription = new SubscriptionData
        {
            Name = createDto.Plan.ToString(),
            Price = PlanPrices[createDto.Plan],
            DurationInMonths = 1,
            Features = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.AddAsync(subscription);
        return MapToDto(created, createDto.UserId, createDto.Plan);
    }

    private static SubscriptionDTO MapToDto(SubscriptionData subscription, int userId, SubscriptionPlan plan)
    {
        var endDate = subscription.CreatedAt.AddMonths(subscription.DurationInMonths);

        return new SubscriptionDTO
        {
            Id = subscription.Id,
            UserId = userId,
            Plan = plan,
            Price = subscription.Price,
            StartDate = subscription.CreatedAt,
            EndDate = endDate,
            IsActive = endDate > DateTime.UtcNow,
            DaysRemaining = Math.Max(0, (int)(endDate - DateTime.UtcNow).TotalDays)
        };
    }

    private static SubscriptionPlan ParsePlan(string? planName)
    {
        return Enum.TryParse<SubscriptionPlan>(planName, true, out var parsedPlan)
            ? parsedPlan
            : SubscriptionPlan.Free;
    }
}
