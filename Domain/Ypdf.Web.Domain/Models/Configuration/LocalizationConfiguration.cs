using System;
using System.Collections.Generic;

namespace Ypdf.Web.Domain.Models.Configuration;

public class LocalizationConfiguration
{
#pragma warning disable SA1305 // Field names should not use Hungarian notation
    public LocalizationConfiguration(
        string cultureName,
        string uiCultureName,
        IEnumerable<string> supportedCultureNames)
    {
        ArgumentException.ThrowIfNullOrEmpty(cultureName, nameof(cultureName));
        ArgumentException.ThrowIfNullOrEmpty(uiCultureName, nameof(uiCultureName));
        ArgumentNullException.ThrowIfNull(supportedCultureNames, nameof(supportedCultureNames));

        CultureName = cultureName;
        UiCultureName = uiCultureName;
        SupportedCultureNames = supportedCultureNames;
    }
#pragma warning restore SA1305 // Field names should not use Hungarian notation

    public string CultureName { get; }
    public string UiCultureName { get; }
    public IEnumerable<string> SupportedCultureNames { get; }
}
