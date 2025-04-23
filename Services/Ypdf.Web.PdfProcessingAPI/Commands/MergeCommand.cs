using System;
using System.Linq;
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
        ISubscriptionInfoService subscriptionInfoService,
        IOutputFilePathService outputFilePathService,
        IRabbitMqProducerService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            nameof(MergeCommand),
            PdfOperationType.Merge,
            subscriptionInfoService ?? throw new ArgumentNullException(nameof(subscriptionInfoService)),
            outputFilePathService ?? throw new ArgumentNullException(nameof(outputFilePathService)),
            rabbitMqSenderService ?? throw new ArgumentNullException(nameof(rabbitMqSenderService)),
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
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
            return System.IO.File.WriteAllBytesAsync(
                outputFilePath,
                request.File!.ToArray());
        });
    }
}
