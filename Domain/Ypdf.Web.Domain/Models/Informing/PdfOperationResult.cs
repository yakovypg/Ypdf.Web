using System;

namespace Ypdf.Web.Domain.Models.Informing;

public class PdfOperationResult
{
    public PdfOperationResult()
    {
        OutputFileName = string.Empty;
    }

    public int UserId { get; set; }
    public PdfOperationType OperationType { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string OutputFileName { get; set; }
}
