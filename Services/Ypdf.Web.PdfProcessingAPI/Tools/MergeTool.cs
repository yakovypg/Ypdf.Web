using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace Ypdf.Web.PdfProcessingAPI.Tools;

public class MergeTool
{
    public MergeTool() { }

    public void Execute(IEnumerable<string> inputPaths, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(inputPaths, nameof(inputPaths));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath, nameof(outputPath));

        using var writer = new PdfWriter(outputPath);
        using var outputDocument = new PdfDocument(writer);

        var merger = new PdfMerger(outputDocument);

        foreach (string path in inputPaths)
        {
            using var reader = new PdfReader(path);
            using var currDoc = new PdfDocument(reader);

            int numOfPages = currDoc.GetNumberOfPages();
            merger.Merge(currDoc, 1, numOfPages);
        }
    }
}
