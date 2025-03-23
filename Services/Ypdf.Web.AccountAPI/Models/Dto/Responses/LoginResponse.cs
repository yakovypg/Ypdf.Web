using System;

namespace Ypdf.Web.AccoutAPI.Models.Dto.Responses;

public class LoginResponse
{
    public LoginResponse(UserDto user, string token)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        User = user;
        Token = token;
    }

    public UserDto User { get; }
    public string Token { get; }
}
