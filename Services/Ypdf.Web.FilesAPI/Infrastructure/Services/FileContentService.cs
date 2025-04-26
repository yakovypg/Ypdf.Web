using System;
using System.IO;

namespace Ypdf.Web.FilesAPI.Infrastructure.Services;

public class FileContentService : IFileContentService
{
    public string GetContentType(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

        string extension = Path
            .GetExtension(filePath)
            .Replace(".", string.Empty, StringComparison.InvariantCulture);

        return extension switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",

            "json" => "application/json",
            "xml" => "application/xml",
            "zip" => "application/zip",
            "pdf" => "application/pdf",

            _ => "application/octet-stream"
        };
    }
}
