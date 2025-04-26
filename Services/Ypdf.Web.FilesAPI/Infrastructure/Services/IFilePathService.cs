namespace Ypdf.Web.FilesAPI.Infrastructure.Services;

public interface IFilePathService
{
    string GetFilesDirectory();
    string GetFilePath(string fileName);
    string GetNextFileName(string extension);
}
