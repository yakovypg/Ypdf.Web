using System;

namespace Ypdf.Web.FilesAPI.Models.Responses;

public class GetOutputFileResponse
{
    public GetOutputFileResponse(string filePath, string fileContentType, string fileDownloadName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(fileContentType, nameof(fileContentType));
        ArgumentException.ThrowIfNullOrWhiteSpace(fileDownloadName, nameof(fileDownloadName));

        FilePath = filePath;
        FileContentType = fileContentType;
        FileDownloadName = fileDownloadName;
    }

    public string FilePath { get; }
    public string FileContentType { get; }
    public string FileDownloadName { get; }
}
