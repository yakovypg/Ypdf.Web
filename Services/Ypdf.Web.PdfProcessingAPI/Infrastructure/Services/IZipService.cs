using System.Collections.Generic;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface IZipService
{
    void ZipFiles(IEnumerable<string> inputFilePaths, string outputFilePath);

    void ZipFiles(
        string intputFilesDirectoryPath,
        string inputFilesPattern,
        string outputFilePath);
}
