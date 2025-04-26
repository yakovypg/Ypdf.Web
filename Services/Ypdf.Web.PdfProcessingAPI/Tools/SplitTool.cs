using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Utils;

namespace Ypdf.Web.PdfProcessingAPI.Tools;

public class SplitTool
{
    public SplitTool(IEnumerable<string> pageRanges)
        : this(pageRanges.Select(t => new PageRange(t))) { }

    public SplitTool(IEnumerable<PageRange> pageRanges)
    {
        ArgumentNullException.ThrowIfNull(pageRanges, nameof(pageRanges));
        PageRanges = pageRanges;
    }

    protected IEnumerable<PageRange> PageRanges { get; }

    public void Execute(string inputPath, string outputPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath, nameof(inputPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath, nameof(outputPath));

        using var splitter = new Splitter(inputPath, outputPath);
        splitter.Split(PageRanges);
    }
}
