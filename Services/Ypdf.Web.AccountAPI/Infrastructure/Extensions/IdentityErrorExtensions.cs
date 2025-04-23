using System;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Extensions;

public static class IdentityErrorExtensions
{
    public static bool DueToPassword(this IdentityError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        string normalizedPrefix = "PASSWORD";
        string normalizedErrorCode = error.Code.ToUpper(CultureInfo.InvariantCulture);

        return normalizedErrorCode.StartsWith(
            normalizedPrefix,
            StringComparison.InvariantCulture);
    }
}
