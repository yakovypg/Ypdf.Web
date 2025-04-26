using System;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.FilesAPI.Infrastructure.Services;

public class InputFilePathService : FilePathService
{
    private readonly IConfiguration _configuration;

    public InputFilePathService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _configuration = configuration;
    }

    public override string GetFilesDirectory()
    {
        return _configuration.GetSection("Storages:InputFiles").Value
            ?? throw new ConfigurationException("Input files directory not specified");
    }
}
