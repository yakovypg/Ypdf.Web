using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Controllers;

[Route("api/output")]
[ApiController]
[ApiVersion("1.0")]
public class OutputController : ControllerBase
{
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ApiResponse<GetOutputFileResponse>> GetOutputFile(
        string name,
        [FromServices] ICommand<GetOutputFileRequest, GetOutputFileResponse> getOutputFileCommand)
    {
        if (getOutputFileCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        var request = new GetOutputFileRequest()
        {
            FileName = name
        };

        GetOutputFileResponse response = await getOutputFileCommand
            .ExecuteAsync(request)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
