using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Requests;

public class SaveFilesRequest
{
    public PdfOperationType OperationType { get; set; }
    public required IReadOnlyCollection<IFormFile> Files { get; set; }
}
