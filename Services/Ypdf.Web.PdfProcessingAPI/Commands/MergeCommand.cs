using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class MergeCommand : BasePdfCommand<MergeRequest>
{
    public MergeCommand(
        IOutputFilePathService outputFilePathService,
        IRabbitMqProducerService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            nameof(MergeCommand),
            PdfOperationType.Merge,
            outputFilePathService ?? throw new ArgumentNullException(nameof(outputFilePathService)),
            rabbitMqSenderService ?? throw new ArgumentNullException(nameof(rabbitMqSenderService)),
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    protected override bool HasAccess(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        return true;
    }

    protected override Task<(DateTimeOffset OperationStart, DateTimeOffset OperationEnd)> GetCommandTask(
        MergeRequest request,
        string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath, nameof(outputFilePath));

        ValidateRequestParameters(request);

        return TimedInvoke.InvokeAsync(() =>
        {
            return System.IO.File.WriteAllTextAsync(
                outputFilePath,
                $"Merge {DateTime.Now.ToLongTimeString()}");
        });
    }
}
