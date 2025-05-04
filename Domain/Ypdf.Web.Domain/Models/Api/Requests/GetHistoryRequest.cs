namespace Ypdf.Web.Domain.Models.Api.Requests;

public class GetHistoryRequest
{
    public int UserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
