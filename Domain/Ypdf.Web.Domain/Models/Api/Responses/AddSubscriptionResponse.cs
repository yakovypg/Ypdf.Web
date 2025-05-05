using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class AddSubscriptionResponse
{
    public AddSubscriptionResponse()
    {
        User = new UserDto();
    }

    public UserDto User { get; set; }
}
