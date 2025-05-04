using System;

namespace Ypdf.Web.Domain.Models.Api.Responses;

public class SaveFilesResponse
{
    public SaveFilesResponse(string outputFileName)
    {
        ArgumentNullException.ThrowIfNull(outputFileName, nameof(outputFileName));
        OutputFileName = outputFileName;
    }

    public string OutputFileName { get; }
}
