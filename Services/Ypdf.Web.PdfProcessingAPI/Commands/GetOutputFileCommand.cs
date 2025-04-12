using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class GetOutputFileCommand : BaseCommand, ICommand<GetOutputFileRequest, GetOutputFileResponse>
{
    private readonly IOutputFilePathService _outputFilePathService;

    public GetOutputFileCommand(
        IOutputFilePathService outputFilePathService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        _outputFilePathService = outputFilePathService;
    }

    public async Task<GetOutputFileResponse> ExecuteAsync(GetOutputFileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(request.FileName, nameof(request.FileName));

        Logger.LogInformation("Trying to get output file: {FileName}", request.FileName);

        string outputFilePath = _outputFilePathService.GetOutputFilePath(request.FileName);

        Logger.LogInformation("Output file path: {OutputFilePath}", outputFilePath);

        byte[] fileBytes = await File
            .ReadAllBytesAsync(outputFilePath)
            .ConfigureAwait(false);

        Logger.LogInformation("Read {ReadBytesCount} bytes from file", fileBytes.Length);

        return new GetOutputFileResponse(fileBytes);
    }
}
