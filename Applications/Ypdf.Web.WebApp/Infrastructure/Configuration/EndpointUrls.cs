using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Ypdf.Web.Domain.Models.Api.Requests;

namespace Ypdf.Web.WebApp.Infrastructure.Configuration;

public static class EndpointUrls
{
#if DEBUG
    public const string AccountApiHost = "localhost:8081";
    public const string FilesApiHost = "localhost:8082";
    public const string HistoryApiHost = "localhost:8083";
#else
    public const string AccountApiHost = "account_api";
    public const string FilesApiHost = "files_api";
    public const string HistoryApiHost = "pdf_operations_history_api";
#endif

    public const string ExternalFilesApiHost = "localhost:8082";

    public static Uri Login => new($"https://{AccountApiHost}/api/account/login/");
    public static Uri Register => new($"https://{AccountApiHost}/api/account/register/");

    public static Uri Merge => new($"https://{FilesApiHost}/api/tool/merge/");
    public static Uri Split => new($"https://{FilesApiHost}/api/tool/split/");

    public static Uri ExternalOutputFileBase => new($"https://{ExternalFilesApiHost}/api/output/");
    public static Uri OutputFileBase => new($"https://{FilesApiHost}/api/output/");
    public static Uri CheckOutputFileBase => new($"https://{FilesApiHost}/api/output/check/");
    public static Uri HistoryBase => new($"https://{HistoryApiHost}/api/history/");

    public static Uri ExternalOutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(ExternalOutputFileBase, fileName);
    }

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
