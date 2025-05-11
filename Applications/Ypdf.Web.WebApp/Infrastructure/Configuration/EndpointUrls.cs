using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Ypdf.Web.Domain.Models.Api.Requests;

namespace Ypdf.Web.WebApp.Infrastructure.Configuration;

public static class EndpointUrls
{
    public static Uri Login => new("https://localhost:8081/api/account/login/");
    public static Uri Register => new("https://localhost:8081/api/account/register/");

    public static Uri Merge => new("https://localhost:8082/api/tool/merge/");
    public static Uri Split => new("https://localhost:8082/api/tool/split/");

    public static Uri OutputFileBase => new("https://localhost:8082/api/output/");
    public static Uri CheckOutputFileBase => new("https://localhost:8082/api/output/check/");
    public static Uri HistoryBase => new("https://localhost:8083/api/history/");

    public static Uri OutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(OutputFileBase, fileName);
    }

    public static Uri CheckOutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(CheckOutputFileBase, fileName);
    }

    public static Uri History(string userId, int pageNumber, int pageSize)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId, nameof(userId));

        var parameters = new Dictionary<string, string?>()
        {
            { nameof(GetHistoryRequest.UserId), userId },
            { nameof(GetHistoryRequest.PageNumber), pageNumber.ToCultureInvariantString() },
            { nameof(GetHistoryRequest.PageSize), pageSize.ToCultureInvariantString() }
        };

        string url = QueryHelpers.AddQueryString(HistoryBase.ToString(), parameters);

        return new Uri(url);
    }
}
