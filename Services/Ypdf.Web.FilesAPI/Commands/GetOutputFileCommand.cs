using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.FilesAPI.Infrastructure.Services;

namespace Ypdf.Web.FilesAPI.Commands;

public class GetOutputFileCommand : BaseCommand, ICommand<GetOutputFileRequest, GetOutputFileResponse>
{
    private readonly OutputFilePathService _outputFilePathService;
    private readonly IFileContentService _fileContentService;

    public GetOutputFileCommand(
        OutputFilePathService outputFilePathService,
        IFileContentService fileContentService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        ArgumentNullException.ThrowIfNull(fileContentService, nameof(fileContentService));

        _outputFilePathService = outputFilePathService;
        _fileContentService = fileContentService;
    }

    public Task<GetOutputFileResponse> ExecuteAsync(GetOutputFileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        ValidateRequestParameters(request);
        Logger.LogInformation("Trying to get output file: {FileName}", request.FileName);

        string filePath = _outputFilePathService.GetFilePath(request.FileName!);
        Logger.LogInformation("Output file path: {FilePath}", filePath);

        ValidateFileExists(filePath);

        string fileContentType = _fileContentService.GetContentType(filePath);

        var response = new GetOutputFileResponse()
        {
            FilePath = filePath,
            FileContentType = fileContentType,
            FileDownloadName = request.FileName!
        };

        return Task.FromResult(response);
    }

    private static void ValidateRequestParameters(GetOutputFileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (string.IsNullOrWhiteSpace(request.FileName))
            throw new BadRequestException("Output file name not specified");
    }

    private static void ValidateFileExists(string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(outputFilePath, nameof(outputFilePath));

        if (!File.Exists(outputFilePath))
            throw new NotFoundException("Output file not found");
    }
}
