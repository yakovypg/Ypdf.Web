using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public interface IMultipleFilesPdfCommandRequest : IPdfCommandRequest
{
    IEnumerable<IEnumerable<byte>>? Files { get; set; }
}
