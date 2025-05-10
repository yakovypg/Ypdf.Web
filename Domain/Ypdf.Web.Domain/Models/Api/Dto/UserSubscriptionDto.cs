using System;

namespace Ypdf.Web.Domain.Models.Api.Dto;

public class UserSubscriptionDto
{
    public SubscriptionDto? Subscription { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
