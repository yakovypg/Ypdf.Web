namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface ITempFileService
{
    string AddTempPostfix(string sourceFilePath);
    string RestoreSourceFilePath(string tempFilePath);
}
