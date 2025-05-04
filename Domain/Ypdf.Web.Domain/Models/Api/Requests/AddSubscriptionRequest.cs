using System;

namespace Ypdf.Web.Domain.Models.Api.Requests;

public class AddSubscriptionRequest
{
    public string? UserEmail { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public TimeSpan Period { get; set; }
}
