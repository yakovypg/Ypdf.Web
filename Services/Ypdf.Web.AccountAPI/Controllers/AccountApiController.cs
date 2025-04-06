using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;

namespace Ypdf.Web.AccoutAPI.Controllers;

[Route("api/account")]
[ApiController]
[ApiVersion("1.0")]
public class AccountApiController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ApiResponse<RegisterUserResponse>> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] ICommand<RegisterUserRequest, RegisterUserResponse> registerUserCommand)
    {
        if (registerUserCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        RegisterUserResponse response = await registerUserCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        [FromServices] ICommand<LoginRequest, LoginResponse> loginCommand)
    {
        if (loginCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        LoginResponse response = await loginCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
