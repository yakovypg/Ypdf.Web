using System;

namespace Ypdf.Web.AccoutAPI.Models.Requests;

public class AddSubscriptionRequest
{
    public string? UserEmail { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public TimeSpan Period { get; set; }
}
