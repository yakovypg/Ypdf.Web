using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.FilesAPI.Infrastructure.Connections;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;
using Ypdf.Web.PdfProcessingAPI.Tools;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class MergeCommand : BasePdfCommand
{
    public MergeCommand(
        PdfOperationResultRabbitMqProducer rabbitMqProducer,
        ITempFileService tempFileService,
        IZipService zipService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            PdfOperationType.Merge,
            rabbitMqProducer ?? throw new ArgumentNullException(nameof(rabbitMqProducer)),
            tempFileService ?? throw new ArgumentNullException(nameof(tempFileService)),
            zipService ?? throw new ArgumentNullException(nameof(zipService)),
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    protected override Task<(DateTimeOffset OperationStart, DateTimeOffset OperationEnd)> GetCommandTask(
        PdfOperationData pdfOperationData)
    {
        ArgumentNullException.ThrowIfNull(pdfOperationData, nameof(pdfOperationData));

        return TimedInvoke.InvokeAsync(() =>
        {
            return Task.Run(() =>
            {
                ExecuteWithTempFile(pdfOperationData.OutputFilePath, outputFilePath =>
                {
                    var mergeTool = new MergeTool();
                    mergeTool.Execute(pdfOperationData.InputFilePaths, outputFilePath);
                });
            });
        });
    }
}
