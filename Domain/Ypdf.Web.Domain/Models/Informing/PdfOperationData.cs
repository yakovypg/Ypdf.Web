using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ypdf.Web.Domain.Infrastructure.Converters;

namespace Ypdf.Web.Domain.Models.Informing;

public class PdfOperationData
{
    public PdfOperationData()
    {
        InputFilePaths = [];
        OutputFilePath = string.Empty;
    }

    [JsonConverter(typeof(EnumJsonConverter<PdfOperationType>))]
    public PdfOperationType OperationType { get; set; }

    public int UserId { get; set; }
    public IEnumerable<string> InputFilePaths { get; set; }
    public string OutputFilePath { get; set; }
}
