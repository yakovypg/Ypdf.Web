using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;

namespace Ypdf.Web.AccoutAPI.Controllers;

[Route("api/subscription")]
[ApiController]
[ApiVersion("1.0")]
public class SubscriptionController : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public async Task<ApiResponse<AddSubscriptionResponse>> AddSubscription(
        [FromQuery] AddSubscriptionRequest request,
        [FromServices] ICommand<AddSubscriptionRequest, AddSubscriptionResponse> addSubscriptionCommand)
    {
        if (request is null || addSubscriptionCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        AddSubscriptionResponse response = await addSubscriptionCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
