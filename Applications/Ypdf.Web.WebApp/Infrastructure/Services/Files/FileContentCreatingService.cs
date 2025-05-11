using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Files;

public class FileContentCreatingService : IFileContentCreatingService
{
    private readonly IBrowserFileSavingService _browserFileSavingService;

    public FileContentCreatingService(IBrowserFileSavingService browserFileSavingService)
    {
        ArgumentNullException.ThrowIfNull(browserFileSavingService, nameof(browserFileSavingService));
        _browserFileSavingService = browserFileSavingService;
    }

    public async Task<MultipartFormDataContent> CreateMultipartFormDataContentAsync(
        IEnumerable<IBrowserFile> files)
    {
        ArgumentNullException.ThrowIfNull(files, nameof(files));

        var content = new MultipartFormDataContent();

        foreach (IBrowserFile file in files)
        {
            await AddFileToContentAsync(content, file)
                .ConfigureAwait(false);
        }

        return content;
    }

    public async Task AddFileToContentAsync(MultipartFormDataContent content, IBrowserFile file)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        ArgumentNullException.ThrowIfNull(file, nameof(file));

        string tempFilePath = await _browserFileSavingService.SaveAsync(file)
            .ConfigureAwait(false);

        var tempFileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var streamContent = new StreamContent(tempFileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
#pragma warning restore CA2000 // Dispose objects before losing scope

        content.Add(streamContent, "files", file.Name);
    }
}
