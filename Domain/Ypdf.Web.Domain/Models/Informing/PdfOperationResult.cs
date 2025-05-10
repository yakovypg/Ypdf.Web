using System;
using System.Text.Json.Serialization;
using Ypdf.Web.Domain.Infrastructure.Converters;

namespace Ypdf.Web.Domain.Models.Informing;

public class PdfOperationResult
{
    public PdfOperationResult()
    {
        OutputFileName = string.Empty;
    }

    [JsonConverter(typeof(EnumJsonConverter<PdfOperationType>))]
    public PdfOperationType OperationType { get; set; }

    public int UserId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string OutputFileName { get; set; }
}
