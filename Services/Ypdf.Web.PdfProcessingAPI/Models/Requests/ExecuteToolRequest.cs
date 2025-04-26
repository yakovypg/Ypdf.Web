using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public class ExecuteToolRequest
{
    public PdfOperationData? PdfOperationData { get; set; }
}
