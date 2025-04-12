using System;
using System.Security;

namespace Ypdf.Web.Domain.Infrastructure.Extensions;

public static class SecureStringExtensions
{
    public static void Enrich(
        this SecureString secureString,
        string text,
        bool makeReadOnly = true)
    {
        ArgumentNullException.ThrowIfNull(secureString, nameof(secureString));
        ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));

        foreach (char c in text)
        {
            secureString.AppendChar(c);
        }

        if (makeReadOnly)
            secureString.MakeReadOnly();
    }
}
