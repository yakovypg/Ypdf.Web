using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;
using Ypdf.Web.PdfProcessingAPI.Services;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class MergeCommand : BasePdfCommand<MergeRequest>
{
    public MergeCommand(
        IRabbitMqSenderService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            nameof(MergeCommand),
            PdfOperationType.Merge,
            rabbitMqSenderService ?? throw new ArgumentNullException(nameof(rabbitMqSenderService)),
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    protected override string GetOutputPath()
    {
        return GetDefaultOutputFilePath();
    }

    protected override Task<(DateTime OperationStart, DateTime OperationEnd)> GetCommandTask(
        MergeRequest request,
        string outputPath)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(outputPath, nameof(outputPath));

        return TimedInvoke.InvokeAsync(() =>
        {
            return System.IO.File.WriteAllTextAsync(outputPath, string.Empty);
        });
    }
}
