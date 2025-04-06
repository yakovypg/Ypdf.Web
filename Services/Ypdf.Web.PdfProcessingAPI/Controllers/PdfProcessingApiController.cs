using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;

namespace Ypdf.Web.AccoutAPI.Controllers;

[Route("api/tool")]
[ApiController]
public class PdfProcessingApiController : ControllerBase
{
    [HttpPost("merge")]
    public async Task<ApiResponse<PdfOperationResponse>> Merge(
        [FromBody] MergeRequest request,
        [FromServices] ICommand<MergeRequest, PdfOperationResponse> mergeCommand)
    {
        if (mergeCommand is null)
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
        if (splitCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        PdfOperationResponse response = await splitCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
