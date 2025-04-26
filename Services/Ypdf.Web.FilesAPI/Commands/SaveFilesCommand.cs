using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Connections;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.FilesAPI.Infrastructure.Connections;
using Ypdf.Web.FilesAPI.Infrastructure.Services;
using Ypdf.Web.FilesAPI.Models.Requests;
using Ypdf.Web.FilesAPI.Models.Responses;

namespace Ypdf.Web.FilesAPI.Commands;

public class SaveFilesCommand : BaseCommand, IProtectedCommand<SaveFilesRequest, SaveFilesResponse>
{
    private readonly InputFilePathService _inputFilePathService;
    private readonly OutputFilePathService _outputFilePathService;
    private readonly ISubscriptionInfoService _subscriptionInfoService;
    private readonly SavedFileRabbitMqProducer _rabbitMqProducer;

    public SaveFilesCommand(
        InputFilePathService inputFilePathService,
        OutputFilePathService outputFilePathService,
        ISubscriptionInfoService subscriptionInfoService,
        SavedFileRabbitMqProducer rabbitMqProducer,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(inputFilePathService, nameof(inputFilePathService));
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        ArgumentNullException.ThrowIfNull(subscriptionInfoService, nameof(subscriptionInfoService));
        ArgumentNullException.ThrowIfNull(rabbitMqProducer, nameof(rabbitMqProducer));

        _inputFilePathService = inputFilePathService;
        _outputFilePathService = outputFilePathService;
        _subscriptionInfoService = subscriptionInfoService;
        _rabbitMqProducer = rabbitMqProducer;
    }

    public async Task<SaveFilesResponse> ExecuteAsync(
        SaveFilesRequest request,
        ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        ValidateRequestParameters(request);

        Logger.LogInformation(
            "Trying to save {FilesCount} file(s) for {OperationName} operation",
            request.Files!.Count,
            request.OperationType);

        await VerifyAccessToOperationAsync(userClaims, request.OperationType)
            .ConfigureAwait(false);

        List<string> inputFilePaths = await SaveInputFilesAsync(request.Files!)
            .ConfigureAwait(false);

        Logger.LogInformation("Input file paths: {@InputPaths}", inputFilePaths);

        int userId = GetUserId(userClaims);

        (string outputFileName, string outputFilePath) = GetOutputFilePath(request.OperationType);
        Logger.LogInformation("Generated output file path: {OutputPath}", outputFilePath);

        var operationData = new PdfOperationData()
        {
            UserId = userId,
            OperationType = request.OperationType,
            InputFilePaths = inputFilePaths,
            OutputFilePath = outputFilePath
        };

        await _rabbitMqProducer
            .SendMessageAsync(operationData)
            .ConfigureAwait(false);

        return new SaveFilesResponse(outputFileName);
    }

    private static void ValidateRequestParameters(SaveFilesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.Files is null)
            throw new BadRequestException("Files not specified");
    }

    private async Task<List<string>> SaveInputFilesAsync(IReadOnlyCollection<IFormFile> files)
    {
        ArgumentNullException.ThrowIfNull(files, nameof(files));

        var filePaths = new List<string>(files.Count);

        foreach (IFormFile file in files)
        {
            string? extension = Path.GetExtension(file.FileName);

            string fileName = _inputFilePathService.GetNextFileName(extension);
            string filePath = _inputFilePathService.GetFilePath(fileName);

            filePaths.Add(filePath);

            using var fileStream = new FileStream(filePath, FileMode.CreateNew);

            await file
                .CopyToAsync(fileStream)
                .ConfigureAwait(false);
        }

        return filePaths;
    }

    private (string FileName, string FilePath) GetOutputFilePath(PdfOperationType operationType)
    {
        string extension = operationType switch
        {
            _ => "pdf"
        };

        string fileName = _outputFilePathService.GetNextFileName(extension);
        string filePath = _outputFilePathService.GetFilePath(fileName);

        return (fileName, filePath);
    }

    private int GetUserId(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        if (!userClaims.Get(NetCoreIdentityClaimNames.Sub, out int userId))
        {
            Logger.LogWarning(
                "Cannot get user id from claims: {UserClaims}",
                userClaims.ToTypeValuePairs());

            throw new ForbiddenException();
        }

        return userId;
    }

    private async Task VerifyAccessToOperationAsync(
        ClaimsPrincipal userClaims,
        PdfOperationType operationType)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        bool hasAccess = await HasUserAccessToOperationAsync(userClaims, operationType)
            .ConfigureAwait(false);

        if (!hasAccess)
        {
            Logger.LogWarning(
                "User {@User} doesn't have access to the {OperationType} operation",
                userClaims.ToTypeValuePairs(),
                operationType);

            throw new ForbiddenException("User doesn't have access to the resource");
        }
    }

    private async Task<bool> HasUserAccessToOperationAsync(
        ClaimsPrincipal userClaims,
        PdfOperationType operationType)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        string? subscriptionName = userClaims.Get(JwtCustomClaimNames.Subscription);

        if (string.IsNullOrEmpty(subscriptionName))
            return false;

        string operationName = operationType.ToString();

        return await _subscriptionInfoService
            .IsOperationAllowedAsync(subscriptionName, operationName)
            .ConfigureAwait(false);
    }
}
