using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Ypdf.Web.Domain.Models.Configuration;

[Serializable]
public class ConfigurationException : Exception
{
    public ConfigurationException() { }

    public ConfigurationException(string? message)
        : base(message) { }

    public ConfigurationException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ConfigurationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    private static string GetDefaultMessage()
    {
        return "Configuration is incorrect.";
    }
}
