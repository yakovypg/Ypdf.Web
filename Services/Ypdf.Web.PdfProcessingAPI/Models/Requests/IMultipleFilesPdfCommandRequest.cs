using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public interface IMultipleFilesPdfCommandRequest
{
    IEnumerable<IEnumerable<byte>>? Files { get; set; }
}
