using System;
using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class AddSubscriptionResponse
{
    public AddSubscriptionResponse(UserDto user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        User = user;
    }

    public UserDto User { get; }
}
