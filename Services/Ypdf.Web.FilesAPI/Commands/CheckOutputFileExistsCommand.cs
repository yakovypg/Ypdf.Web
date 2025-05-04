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

public class CheckOutputFileExistsCommand : BaseCommand, ICommand<CheckOutputFileExistsRequest, CheckOutputFileExistsResponse>
{
    private readonly OutputFilePathService _outputFilePathService;

    public CheckOutputFileExistsCommand(
        OutputFilePathService outputFilePathService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        _outputFilePathService = outputFilePathService;
    }

    public Task<CheckOutputFileExistsResponse> ExecuteAsync(CheckOutputFileExistsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        ValidateRequestParameters(request);
        Logger.LogInformation("Check if output file {FileName} exists", request.FileName);

        string outputFilePath = _outputFilePathService.GetFilePath(request.FileName!);
        Logger.LogInformation("Output file path: {OutputFilePath}", outputFilePath);

        bool fileExists = File.Exists(outputFilePath);

        if (fileExists)
            Logger.LogInformation("Output file {FileName} exists", request.FileName);
        else
            Logger.LogInformation("Output file {FileName} not exists", request.FileName);

        var response = new CheckOutputFileExistsResponse(fileExists);

        return Task.FromResult(response);
    }

    private static void ValidateRequestParameters(CheckOutputFileExistsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (string.IsNullOrWhiteSpace(request.FileName))
            throw new BadRequestException("Output file name not specified");
    }
}
