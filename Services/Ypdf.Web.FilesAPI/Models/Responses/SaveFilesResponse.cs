using System;

namespace Ypdf.Web.FilesAPI.Models.Responses;

public class SaveFilesResponse
{
    public SaveFilesResponse(string outputFileName)
    {
        ArgumentNullException.ThrowIfNull(outputFileName, nameof(outputFileName));
        OutputFileName = outputFileName;
    }

    public string OutputFileName { get; }
}
