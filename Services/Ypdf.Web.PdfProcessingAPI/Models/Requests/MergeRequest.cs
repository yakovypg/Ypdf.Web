using System;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public class MergeRequest : ISingleFilePdfCommandRequest
{
    public Guid UserId { get; set; }
    public object? File { get; set; }
}
