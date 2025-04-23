using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Requests;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Responses;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Controllers;

[Route("api/history")]
[ApiController]
[ApiVersion("1.0")]
public class HistoryController : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ApiResponse<GetHistoryResponse>> GetHistory(
        int id,
        [FromServices] IProtectedCommand<GetHistoryRequest, GetHistoryResponse> getHistoryCommand)
    {
        if (getHistoryCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        var request = new GetHistoryRequest()
        {
            UserId = id
        };

        GetHistoryResponse response = await getHistoryCommand
            .ExecuteAsync(request, User)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
