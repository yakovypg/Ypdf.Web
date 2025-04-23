using System;

namespace Ypdf.Web.AccoutAPI.Models.Dto;

public class UserSubscriptionDto
{
    public SubscriptionType SubscriptionType { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
