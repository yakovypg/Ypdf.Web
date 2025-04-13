using System.Net;

namespace Ypdf.Web.Domain.Models.Api;

public class ApiResponse<T>
{
    public ApiResponse(T? result, HttpStatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
    }

    public T? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
