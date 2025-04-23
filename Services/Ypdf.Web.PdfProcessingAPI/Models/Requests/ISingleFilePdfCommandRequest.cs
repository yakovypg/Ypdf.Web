namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public interface ISingleFilePdfCommandRequest : IPdfCommandRequest
{
    object? File { get; set; }
}
