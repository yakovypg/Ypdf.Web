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
    public static Uri HistoryBase => new("https://localhost:8083/api/history/");

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
