using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.WebApp.Infrastructure.Configuration;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Api;

public class EndpointUrlsService : IEndpointUrlsService
{
    public EndpointUrlsService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        DeploymentType deployment = configuration.GetValue<DeploymentType>("Deployment");

        switch (deployment)
        {
            case DeploymentType.Local:
                AccountApiHost = "localhost:8081";
                FilesApiHost = "localhost:8082";
                HistoryApiHost = "localhost:8083";
                ExternalFilesApiHost = "localhost:8082";
                break;

            case DeploymentType.Docker:
                AccountApiHost = "account-api";
                FilesApiHost = "files-api";
                HistoryApiHost = "pdf-operations-history-api";
                ExternalFilesApiHost = "localhost:8082";
                break;

            case DeploymentType.Kubernetes:
                AccountApiHost = "account-api.ypdf.svc.cluster.local:8081";
                FilesApiHost = "files-api.ypdf.svc.cluster.local:8082";
                HistoryApiHost = "pdf-operations-history-api.ypdf.svc.cluster.local:8083";
                ExternalFilesApiHost = "localhost:8082";
                break;

            default:
                throw new NotSupportedException($"Deployment {deployment} not supported");
        }
    }

    public string AccountApiHost { get; }
    public string FilesApiHost { get; }
    public string HistoryApiHost { get; }
    public string ExternalFilesApiHost { get; }

    public Uri Login => new($"https://{AccountApiHost}/api/account/login/");
    public Uri Register => new($"https://{AccountApiHost}/api/account/register/");

    public Uri Merge => new($"https://{FilesApiHost}/api/tool/merge/");
    public Uri Split => new($"https://{FilesApiHost}/api/tool/split/");

    public Uri ExternalOutputFileBase => new($"https://{ExternalFilesApiHost}/api/output/");
    public Uri OutputFileBase => new($"https://{FilesApiHost}/api/output/");
    public Uri CheckOutputFileBase => new($"https://{FilesApiHost}/api/output/check/");
    public Uri HistoryBase => new($"https://{HistoryApiHost}/api/history/");

    public Uri ExternalOutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(ExternalOutputFileBase, fileName);
    }

    public Uri OutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(OutputFileBase, fileName);
    }

    public Uri CheckOutputFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        return new Uri(CheckOutputFileBase, fileName);
    }

    public Uri History(string userId, int pageNumber, int pageSize)
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
