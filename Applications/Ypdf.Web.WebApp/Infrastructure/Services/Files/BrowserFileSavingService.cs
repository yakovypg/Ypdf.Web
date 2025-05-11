using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Files;

public class BrowserFileSavingService : IBrowserFileSavingService
{
    private const long MaxFileSizeBytes = 1024 * 1024 * 1024 * 4L; // 4 GB

    public async Task<string> SaveAsync(IBrowserFile browserFile)
    {
        ArgumentNullException.ThrowIfNull(browserFile, nameof(browserFile));

        VerifyFileSize(browserFile);

        Stream browserFileStream = browserFile.OpenReadStream(MaxFileSizeBytes);
        string browserFileExtension = Path.GetExtension(browserFile.Name);

        string tempFilePath = Path.GetTempFileName();
        tempFilePath = Path.ChangeExtension(tempFilePath, browserFileExtension);

        using var targetStream = new FileStream(tempFilePath, FileMode.Create);

        await browserFileStream
            .CopyToAsync(targetStream)
            .ConfigureAwait(false);

        return tempFilePath;
    }

    private static void VerifyFileSize(IBrowserFile browserFile)
    {
        ArgumentNullException.ThrowIfNull(browserFile, nameof(browserFile));

        ArgumentOutOfRangeException.ThrowIfGreaterThan(
            browserFile.Size, MaxFileSizeBytes, nameof(browserFile.Size));
    }
}
