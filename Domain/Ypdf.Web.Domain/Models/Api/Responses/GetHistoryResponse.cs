using System.Collections.Generic;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class GetHistoryResponse
{
    public GetHistoryResponse()
    {
        PdfOperationResults = [];
    }

    public int MinPage { get; set; }
    public int MaxPage { get; set; }
    public IEnumerable<PdfOperationResult> PdfOperationResults { get; set; }
}
