using System;
using System.Collections.Generic;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Models.Dto.Responses;

public class GetHistoryResponse
{
    public GetHistoryResponse(IEnumerable<PdfOperationResult> pdfOperationResults)
    {
        ArgumentNullException.ThrowIfNull(pdfOperationResults, nameof(pdfOperationResults));
        PdfOperationResults = pdfOperationResults;
    }

    public IEnumerable<PdfOperationResult> PdfOperationResults { get; }
}
