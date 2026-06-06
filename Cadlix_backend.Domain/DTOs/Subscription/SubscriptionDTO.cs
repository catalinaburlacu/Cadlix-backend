using System;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.Domain.DTOs;

public class SubscriptionDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int DaysRemaining { get; set; }
}
