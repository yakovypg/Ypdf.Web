using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;

namespace Ypdf.Web.Domain.Models.Api.Exceptions;

[Serializable]
public class ConflictException : ApiException
{
    public ConflictException()
        : base(GetDefaultMessage(), HttpStatusCode.Conflict) { }

    public ConflictException(string? message)
        : base(message ?? GetDefaultMessage(), HttpStatusCode.Conflict) { }

    public ConflictException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), HttpStatusCode.Conflict, innerException) { }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ConflictException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
    }

    private static string GetDefaultMessage()
    {
        return "Conflict occured";
    }
}
