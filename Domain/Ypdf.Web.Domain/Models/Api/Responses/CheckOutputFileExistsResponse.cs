namespace Ypdf.Web.Domain.Models.Api.Responses;

public class CheckOutputFileExistsResponse
{
    public CheckOutputFileExistsResponse(bool exists)
    {
        Exists = exists;
    }

    public bool Exists { get; }
}
