using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Ypdf.Web.Domain.Infrastructure.Converters;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.Domain.Models.Api.Requests;

public class SaveFilesRequest
{
    [JsonConverter(typeof(EnumJsonConverter<PdfOperationType>))]
    public PdfOperationType OperationType { get; set; }
    public IReadOnlyCollection<IFormFile>? Files { get; set; }
}
