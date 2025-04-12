using System;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;

public class PdfOperationResponse
{
    public PdfOperationResponse(string outputFileName, PdfOperationResult operationResult)
    {
        ArgumentNullException.ThrowIfNull(outputFileName, nameof(outputFileName));
        ArgumentNullException.ThrowIfNull(operationResult, nameof(operationResult));

        OutputFileName = outputFileName;
        OperationResult = operationResult;
    }

    public string OutputFileName { get; }
    public PdfOperationResult OperationResult { get; }
}
