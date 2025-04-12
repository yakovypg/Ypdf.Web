using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class OutputFilePathService : IOutputFilePathService
{
    private readonly IConfiguration _configuration;

    public OutputFilePathService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _configuration = configuration;
    }

    public string GetOutputFilePath(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        string outputFilesDirectory = _configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");

        return Path.Combine(outputFilesDirectory, fileName);
    }
}
