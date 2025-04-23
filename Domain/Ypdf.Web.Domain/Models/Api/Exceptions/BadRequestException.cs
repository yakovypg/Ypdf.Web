using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;

namespace Ypdf.Web.Domain.Models.Api.Exceptions;

[Serializable]
public class BadRequestException : ApiException
{
    public BadRequestException()
        : base(GetDefaultMessage(), HttpStatusCode.BadRequest) { }

    public BadRequestException(string? message)
        : base(message ?? GetDefaultMessage(), HttpStatusCode.BadRequest) { }

    public BadRequestException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), HttpStatusCode.BadRequest, innerException) { }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected BadRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
    }

    private static string GetDefaultMessage()
    {
        return "Request is incorrect";
    }
}
