using System;
using System.Collections.Generic;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class GetHistoryResponse
{
    public GetHistoryResponse(
        int minPage,
        int maxPage,
        IEnumerable<PdfOperationResult> pdfOperationResults)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minPage, nameof(minPage));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minPage, maxPage, nameof(minPage));
        ArgumentNullException.ThrowIfNull(pdfOperationResults, nameof(pdfOperationResults));

        MinPage = minPage;
        MaxPage = maxPage;
        PdfOperationResults = pdfOperationResults;
    }

    public int MinPage { get; }
    public int MaxPage { get; }
    public IEnumerable<PdfOperationResult> PdfOperationResults { get; }
}
