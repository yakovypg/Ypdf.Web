using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public class MergeRequest : ISingleFilePdfCommandRequest
{
    public IEnumerable<byte>? File { get; set; }
}
