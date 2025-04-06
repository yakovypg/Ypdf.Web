using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.FileStoringAPI.Models.Dto.Requests;
using Ypdf.Web.FileStoringAPI.Models.Dto.Responses;

namespace Ypdf.Web.AccoutAPI.Controllers;

[Route("api/output")]
[ApiController]
[ApiVersion("1.0")]
public class FileStoringApiController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<GetOutputFileResponse>> GetOutputFile(
        Guid id,
        [FromServices] ICommand<GetOutputFileRequest, GetOutputFileResponse> getOutputFileCommand)
    {
        if (getOutputFileCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        var request = new GetOutputFileRequest()
        {
            Id = id
        };

        GetOutputFileResponse response = await getOutputFileCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
