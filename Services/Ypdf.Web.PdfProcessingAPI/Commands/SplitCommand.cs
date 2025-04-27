using System;
using System.IO;
using System.Linq;
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

public class SplitCommand : BasePdfCommand
{
    public SplitCommand(
        PdfOperationResultRabbitMqProducer rabbitMqProducer,
        ITempFileService tempFileService,
        IZipService zipService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            PdfOperationType.Split,
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

        string inputFilePath = pdfOperationData.InputFilePaths.First();
        string outputFilePath = pdfOperationData.OutputFilePath;

        return TimedInvoke.InvokeAsync(() =>
        {
            return Task.Run(() =>
            {
                ExecuteAndZip(inputFilePath, outputFilePath, (input, output) =>
                {
                    var splitTool = new SplitTool(["1-1"]);
                    splitTool.Execute(input, output);
                });
            });
        });
    }
}
