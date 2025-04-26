using System;
using System.IO;

namespace Ypdf.Web.FilesAPI.Infrastructure.Services;

public abstract class FilePathService : IFilePathService
{
    protected FilePathService()
    {
    }

    public abstract string GetFilesDirectory();

    public string GetFilePath(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        string filesDirectory = GetFilesDirectory();

        return Path.Combine(filesDirectory, fileName);
    }

    public string GetNextFileName(string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension, nameof(extension));

        string fileName;
        string filePath;

        do
        {
            Guid fileNameWithoutExtension = Guid.NewGuid();

            fileName = $"{fileNameWithoutExtension}.{extension}";
            filePath = GetFilePath(fileName);
        }
        while (File.Exists(filePath));

        return fileName;
    }
}
