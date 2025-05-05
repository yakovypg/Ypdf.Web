using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class LoginResponse
{
    public LoginResponse()
    {
        User = new UserDto();
        Token = string.Empty;
    }

    public UserDto User { get; set; }
    public string Token { get; set; }
}
