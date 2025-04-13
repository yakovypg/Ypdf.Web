namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

public interface ISingleFilePdfCommandRequest : IPdfCommandRequest
{
    object? File { get; set; }
}
