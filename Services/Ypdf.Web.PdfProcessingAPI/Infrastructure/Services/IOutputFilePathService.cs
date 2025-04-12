namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface IOutputFilePathService
{
    string GetOutputFilePath(string fileName);
}
