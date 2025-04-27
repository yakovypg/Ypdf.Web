using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class ZipService : IZipService
{
    public void ZipFiles(
        string intputFilesDirectoryPath,
        string inputFilesPattern,
        string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(intputFilesDirectoryPath, nameof(intputFilesDirectoryPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(inputFilesPattern, nameof(inputFilesPattern));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath, nameof(outputFilePath));

        outputFilePath = AddExtensionIfNeeded(outputFilePath);
        string[] filePaths = Directory.GetFiles(intputFilesDirectoryPath, inputFilesPattern);

        ZipFiles(filePaths, outputFilePath);
    }

    public void ZipFiles(IEnumerable<string> inputFilePaths, string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(inputFilePaths, nameof(inputFilePaths));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath, nameof(outputFilePath));

        IReadOnlyCollection<string> inputFilePathCollection = inputFilePaths.ToArray();

        if (inputFilePathCollection.Count == 0)
            throw new ArgumentException("No files specified", nameof(inputFilePaths));

        using var zipFileStream = new FileStream(outputFilePath, FileMode.Create);
        using var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Create);

        foreach (string filePath in inputFilePathCollection)
        {
            string fileName = Path.GetFileName(filePath);
            zipArchive.CreateEntryFromFile(filePath, fileName);
        }
    }

    private static string AddExtensionIfNeeded(string outputFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFileName, nameof(outputFileName));

        const string expectedExtension = ".zip";

        return !outputFileName.EndsWith(expectedExtension, StringComparison.InvariantCulture)
            ? $"{outputFileName}{expectedExtension}"
            : outputFileName;
    }
}
