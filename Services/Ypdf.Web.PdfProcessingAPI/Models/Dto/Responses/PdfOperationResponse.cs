using System;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;

public class PdfOperationResponse
{
    public PdfOperationResponse(PdfOperationResult operationResult)
    {
        ArgumentNullException.ThrowIfNull(operationResult, nameof(operationResult));
        OperationResult = operationResult;
    }

    public PdfOperationResult OperationResult { get; }
}
