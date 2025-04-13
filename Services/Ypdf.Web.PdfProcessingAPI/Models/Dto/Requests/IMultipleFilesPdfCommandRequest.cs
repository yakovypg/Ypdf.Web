using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

public interface IMultipleFilesPdfCommandRequest : IPdfCommandRequest
{
    IReadOnlyCollection<object>? Files { get; set; }
}
