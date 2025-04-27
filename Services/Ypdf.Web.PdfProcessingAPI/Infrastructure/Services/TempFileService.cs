using System;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class TempFileService : ITempFileService
{
    private const string TempFilePostfix = ".tmp";

    public string AddTempPostfix(string sourceFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath, nameof(sourceFilePath));
        return $"{sourceFilePath}{TempFilePostfix}";
    }

    public string RestoreSourceFilePath(string tempFilePath)
    {
        ArgumentNullException.ThrowIfNull(tempFilePath, nameof(tempFilePath));

        return tempFilePath.EndsWith(TempFilePostfix, StringComparison.InvariantCulture)
            ? tempFilePath.Remove(tempFilePath.Length - TempFilePostfix.Length)
            : throw new ArgumentException("Invalid file path", nameof(tempFilePath));
    }
}
