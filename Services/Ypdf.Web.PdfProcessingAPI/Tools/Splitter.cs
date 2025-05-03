using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace Ypdf.Web.PdfProcessingAPI.Tools;

public class Splitter : PdfSplitter, IDisposable
{
    private const long DefaultSplittingPartSizeInBytes = 3 * 1024 * 1024;

    private readonly PdfReader? _sourceDocumentReader;
    private readonly PdfDocument? _sourceDocument;

    private bool _isDisposed;

    public Splitter(string inputPath, string outputDirectoryPath)
        : this(new PdfReader(inputPath))
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath, nameof(inputPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectoryPath, nameof(outputDirectoryPath));

        InputFileName = Path.GetFileNameWithoutExtension(inputPath);
        OutputDirectoryPath = outputDirectoryPath;
    }

    private Splitter(PdfReader sourceDocumentReader)
        : this(new PdfDocument(sourceDocumentReader))
    {
        _sourceDocumentReader = sourceDocumentReader;
    }

    private Splitter(PdfDocument sourceDocument)
        : base(sourceDocument ?? throw new ArgumentNullException(nameof(sourceDocument)))
    {
        _sourceDocument = sourceDocument;
    }

    ~Splitter()
    {
        Dispose(false);
    }

    protected string InputFileName { get; } = string.Empty;
    protected string OutputDirectoryPath { get; } = string.Empty;

    public void Split(IEnumerable<PageRange> pageRanges)
    {
        ArgumentNullException.ThrowIfNull(pageRanges, nameof(pageRanges));

        List<PageRange> pageRangesList = new(pageRanges);
        IList<PdfDocument> documents = ExtractPageRanges(pageRangesList);

        foreach (PdfDocument document in documents)
        {
            document.Close();
        }
    }

    public void Split(long splittingPartSize = DefaultSplittingPartSizeInBytes)
    {
        IList<PdfDocument> documents = SplitBySize(splittingPartSize);

        foreach (PdfDocument document in documents)
        {
            document.Close();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _sourceDocumentReader?.Close();
            _sourceDocument?.Close();

            _isDisposed = true;
        }
    }

    protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
    {
        ArgumentNullException.ThrowIfNull(documentPageRange, nameof(documentPageRange));

        IList<int> pages = documentPageRange.GetQualifyingPageNums(int.MaxValue);
        string pageRange = pages.Count > 0 ? $"{pages[0]}" : "0";

        if (pages.Count > 1)
        {
            int lastPage = pages[pages.Count - 1];
            pageRange += $"-{lastPage}";
        }

        string fileName = $"{InputFileName}_{pageRange}.pdf";
        string filePath = Path.Combine(OutputDirectoryPath, fileName);

        return new PdfWriter(filePath);
    }
}
