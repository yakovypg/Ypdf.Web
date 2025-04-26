namespace Ypdf.Web.FilesAPI.Models.Responses;

public class CheckOutputFileExistsResponse
{
    public CheckOutputFileExistsResponse(bool exists)
    {
        Exists = exists;
    }

    public bool Exists { get; }
}
