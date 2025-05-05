namespace Ypdf.Web.Domain.Models.Api.Responses;

public class GetOutputFileResponse
{
    public GetOutputFileResponse()
    {
        FilePath = string.Empty;
        FileContentType = string.Empty;
        FileDownloadName = string.Empty;
    }

    public string FilePath { get; set; }
    public string FileContentType { get; set; }
    public string FileDownloadName { get; set; }
}
