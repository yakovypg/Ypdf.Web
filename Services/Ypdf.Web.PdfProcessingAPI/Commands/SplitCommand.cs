using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class SplitCommand : BasePdfCommand<SplitRequest>
{
    public SplitCommand(
        IRabbitMqProducerService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            nameof(SplitCommand),
            PdfOperationType.Split,
            rabbitMqSenderService ?? throw new ArgumentNullException(nameof(rabbitMqSenderService)),
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    protected override Task<(DateTime OperationStart, DateTime OperationEnd)> GetCommandTask(
        SplitRequest request,
        string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(outputFilePath, nameof(outputFilePath));

        return TimedInvoke.InvokeAsync(() =>
        {
            return System.IO.File.WriteAllTextAsync(outputFilePath, string.Empty);
        });
    }
}
