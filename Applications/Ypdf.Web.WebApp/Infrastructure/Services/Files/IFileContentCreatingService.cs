using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Files;

public interface IFileContentCreatingService
{
    Task<MultipartFormDataContent> CreateMultipartFormDataContentAsync(
        IEnumerable<IBrowserFile> files);

    Task AddFileToContentAsync(MultipartFormDataContent content, IBrowserFile file);
}
