namespace Ypdf.Web.Domain.Models.Api.Responses;

public class SaveFilesResponse
{
    public SaveFilesResponse()
    {
        OutputFileName = string.Empty;
    }

    public string OutputFileName { get; set; }
}
