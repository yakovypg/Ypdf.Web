using System;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.FilesAPI.Infrastructure.Services;

public class OutputFilePathService : FilePathService
{
    private readonly IConfiguration _configuration;

    public OutputFilePathService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _configuration = configuration;
    }

    public override string GetFilesDirectory()
    {
        return _configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");
    }
}
