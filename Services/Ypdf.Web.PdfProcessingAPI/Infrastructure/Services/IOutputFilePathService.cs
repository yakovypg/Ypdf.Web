namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface IOutputFilePathService
{
    string GetOutputFilesDirectory();
    string GetOutputFilePath(string fileName);
    string GetNextOutputFileName(string extension);
}
