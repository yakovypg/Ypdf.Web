using System;

namespace Ypdf.Web.AccoutAPI.Models.Dto.Responses;

public class RegisterUserResponse
{
    public RegisterUserResponse(UserDto user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        User = user;
    }

    public UserDto User { get; }
}
