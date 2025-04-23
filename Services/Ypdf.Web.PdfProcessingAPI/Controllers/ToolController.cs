using System.Net;
using System.Threading.Tasks;
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
    public async Task<ApiResponse<PdfOperationResponse>> Merge(
        [FromBody] MergeRequest request,
        [FromServices] ICommand<MergeRequest, PdfOperationResponse> mergeCommand)
    {
        if (request is null || mergeCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        PdfOperationResponse response = await mergeCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }

    [HttpPost("split")]
    public async Task<ApiResponse<PdfOperationResponse>> Split(
        [FromBody] SplitRequest request,
        [FromServices] ICommand<SplitRequest, PdfOperationResponse> splitCommand)
    {
        if (request is null || splitCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        PdfOperationResponse response = await splitCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
