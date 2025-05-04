namespace Ypdf.Web.Domain.Models.Api.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string? Nickname { get; set; }
    public UserSubscriptionDto? Subscription { get; set; }
}
