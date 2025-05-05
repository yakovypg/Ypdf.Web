using System.Net;
using System.Text.Json.Serialization;
using Ypdf.Web.Domain.Infrastructure.Converters;

namespace Ypdf.Web.Domain.Models.Api;

public class ApiResponse<T>
{
    public ApiResponse()
        : this(default, HttpStatusCode.InternalServerError)
    {
    }

    public ApiResponse(T? result, HttpStatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
    }

    public T? Result { get; set; }

    [JsonConverter(typeof(EnumJsonConverter<HttpStatusCode>))]
    public HttpStatusCode StatusCode { get; set; }
}
