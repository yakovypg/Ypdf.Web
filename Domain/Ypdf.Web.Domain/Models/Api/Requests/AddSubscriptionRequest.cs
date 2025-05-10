using System;
using System.Text.Json.Serialization;
using Ypdf.Web.Domain.Infrastructure.Converters;

namespace Ypdf.Web.Domain.Models.Api.Requests;

public class AddSubscriptionRequest
{
    public string? UserEmail { get; set; }

    [JsonConverter(typeof(EnumJsonConverter<SubscriptionType>))]
    public SubscriptionType SubscriptionType { get; set; }

    public TimeSpan Period { get; set; }
}
