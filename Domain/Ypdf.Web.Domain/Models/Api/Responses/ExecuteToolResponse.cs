using System;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class ExecuteToolResponse
{
    public ExecuteToolResponse(PdfOperationResult operationResult)
    {
        ArgumentNullException.ThrowIfNull(operationResult, nameof(operationResult));
        OperationResult = operationResult;
    }

    public PdfOperationResult OperationResult { get; set; }
}
