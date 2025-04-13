using System;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

public class SplitRequest : ISingleFilePdfCommandRequest
{
    public Guid UserId { get; set; }
    public object? File { get; set; }
}
