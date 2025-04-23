using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public class SplitRequest : ISingleFilePdfCommandRequest
{
    public IEnumerable<byte>? File { get; set; }
}
