using System;

namespace Ypdf.Web.Domain.Models.Informing;

public class PdfOperationResult
{
    public Guid UserId { get; set; }
    public PdfOperationType OperationType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
