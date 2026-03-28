using System;
using Cadlix_backend.Domain.Enum;

namespace Cadlix_backend.Domain.DTOs;

public class CreateSubscriptionDTO
{
    public int UserId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public string PaymentToken { get; set; } = string.Empty;
}
