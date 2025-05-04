using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.FilesAPI.Controllers;

[Route("api/tool")]
[ApiController]
[ApiVersion("1.0")]
public class ToolController : ControllerBase
{
    [HttpPost("merge")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ApiResponse<SaveFilesResponse>> Merge(
        IReadOnlyCollection<IFormFile> files,
        [FromServices] IProtectedCommand<SaveFilesRequest, SaveFilesResponse> saveFilesCommand)
    {
        return await SaveFiles(files, PdfOperationType.Merge, saveFilesCommand)
            .ConfigureAwait(false);
    }

    [HttpPost("split")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ApiResponse<SaveFilesResponse>> Split(
        IReadOnlyCollection<IFormFile> files,
        [FromServices] IProtectedCommand<SaveFilesRequest, SaveFilesResponse> saveFilesCommand)
    {
        return await SaveFiles(files, PdfOperationType.Split, saveFilesCommand)
            .ConfigureAwait(false);
    }

    private async Task<ApiResponse<SaveFilesResponse>> SaveFiles(
        IReadOnlyCollection<IFormFile> files,
        PdfOperationType operationType,
        IProtectedCommand<SaveFilesRequest, SaveFilesResponse> saveFilesCommand)
    {
        if (saveFilesCommand is null)
            return new(null, HttpStatusCode.InternalServerError);

        if (files is null)
            return new(null, HttpStatusCode.BadRequest);

        var saveFilesRequest = new SaveFilesRequest()
        {
            OperationType = operationType,
            Files = files
        };

        SaveFilesResponse response = await saveFilesCommand
            .ExecuteAsync(saveFilesRequest, User)
            .ConfigureAwait(false);

        return new(response, HttpStatusCode.OK);
    }
}
