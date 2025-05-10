using System.Text.Json.Serialization;
using Ypdf.Web.Domain.Infrastructure.Converters;

namespace Ypdf.Web.Domain.Models.Api.Dto;

public class SubscriptionDto
{
    [JsonConverter(typeof(EnumJsonConverter<SubscriptionType>))]
    public SubscriptionType SubscriptionType { get; set; }
}
