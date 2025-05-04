using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;

namespace Ypdf.Web.FilesAPI.Controllers;

[Route("api/output")]
[ApiController]
[ApiVersion("1.0")]
public class OutputController : ControllerBase
{
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<FileResult> GetOutputFile(
        string name,
        [FromServices] ICommand<GetOutputFileRequest, GetOutputFileResponse> getOutputFileCommand)
    {
        ArgumentNullException.ThrowIfNull(getOutputFileCommand, nameof(getOutputFileCommand));

        var request = new GetOutputFileRequest()
        {
            FileName = name
        };

        GetOutputFileResponse response = await getOutputFileCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return PhysicalFile(
            response.FilePath,
            response.FileContentType,
            response.FileDownloadName);
    }

    [HttpGet("check/{name}")]
    [AllowAnonymous]
    public async Task<ApiResponse<CheckOutputFileExistsResponse>> CheckOutputFileExists(
        string name,
        [FromServices] ICommand<CheckOutputFileExistsRequest, CheckOutputFileExistsResponse> checkOutputFileExistsCommand)
    {
        if (checkOutputFileExistsCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        var request = new CheckOutputFileExistsRequest()
        {
            FileName = name
        };

        CheckOutputFileExistsResponse response = await checkOutputFileExistsCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
