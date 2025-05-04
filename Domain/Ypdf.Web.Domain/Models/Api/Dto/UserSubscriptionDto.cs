using System;

namespace Ypdf.Web.Domain.Models.Api.Dto;

public class UserSubscriptionDto
{
    public SubscriptionType SubscriptionType { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
