using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class ExecuteToolResponse
{
    public ExecuteToolResponse()
    {
        OperationResult = new PdfOperationResult();
    }

    public PdfOperationResult OperationResult { get; set; }
}
