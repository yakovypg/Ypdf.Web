using System;
using Ypdf.Web.AccoutAPI.Models.Dto;

namespace Ypdf.Web.AccoutAPI.Models.Responses;

public class AddSubscriptionResponse
{
    public AddSubscriptionResponse(UserDto user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        User = user;
    }

    public UserDto User { get; }
}
