using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class RegisterUserResponse
{
    public RegisterUserResponse()
    {
        User = new UserDto();
    }

    public UserDto User { get; set; }
}
