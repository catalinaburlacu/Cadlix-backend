using System;
using Cadlix_backend.BusinessLogic.Enum;

namespace Cadlix_backend.BusinessLogic.DTOs;

public class CreateSubscriptionDTO
{
    public int UserId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public string PaymentToken { get; set; }
}
