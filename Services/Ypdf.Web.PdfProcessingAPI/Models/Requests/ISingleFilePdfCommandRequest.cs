using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public interface ISingleFilePdfCommandRequest
{
    IEnumerable<byte>? File { get; set; }
}
