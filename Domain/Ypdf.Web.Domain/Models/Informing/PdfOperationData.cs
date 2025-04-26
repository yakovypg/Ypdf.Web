using System.Collections.Generic;

namespace Ypdf.Web.Domain.Models.Informing;

public class PdfOperationData
{
    public PdfOperationData()
    {
        InputFilePaths = [];
        OutputFilePath = string.Empty;
    }

    public int UserId { get; set; }
    public PdfOperationType OperationType { get; set; }
    public IEnumerable<string> InputFilePaths { get; set; }
    public string OutputFilePath { get; set; }
}
