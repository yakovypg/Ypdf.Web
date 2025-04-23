using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Controllers;

[Route("api/tool")]
[ApiController]
[ApiVersion("1.0")]
public class ToolController : ControllerBase
{
    [HttpPost("merge")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ApiResponse<PdfOperationResponse>> Merge(
        [FromBody] MergeRequest request,
        [FromServices] IProtectedCommand<MergeRequest, PdfOperationResponse> mergeCommand)
    {
        if (request is null || mergeCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        PdfOperationResponse response = await mergeCommand
            .ExecuteAsync(request, User)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }

    [HttpPost("split")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ApiResponse<PdfOperationResponse>> Split(
        [FromBody] SplitRequest request,
        [FromServices] IProtectedCommand<SplitRequest, PdfOperationResponse> splitCommand)
    {
        if (request is null || splitCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        PdfOperationResponse response = await splitCommand
            .ExecuteAsync(request, User)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
