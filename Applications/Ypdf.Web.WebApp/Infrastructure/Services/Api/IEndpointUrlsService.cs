using System;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Api;

public interface IEndpointUrlsService
{
    public string AccountApiHost { get; }
    public string FilesApiHost { get; }
    public string HistoryApiHost { get; }
    public string ExternalFilesApiHost { get; }

    public Uri Login { get; }
    public Uri Register { get; }

    public Uri Merge { get; }
    public Uri Split { get; }

    public Uri ExternalOutputFileBase { get; }
    public Uri OutputFileBase { get; }
    public Uri CheckOutputFileBase { get; }
    public Uri HistoryBase { get; }

    public Uri ExternalOutputFile(string fileName);
    public Uri OutputFile(string fileName);
    public Uri CheckOutputFile(string fileName);
    public Uri History(string userId, int pageNumber, int pageSize);
}
