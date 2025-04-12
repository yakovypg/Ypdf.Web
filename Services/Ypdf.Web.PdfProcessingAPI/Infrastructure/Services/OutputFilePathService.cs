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

    public string GetOutputFilesDirectory()
    {
        return _configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");
    }

    public string GetOutputFilePath(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        string outputFilesDirectory = GetOutputFilesDirectory();

        return Path.Combine(outputFilesDirectory, fileName);
    }

    public string GetNextOutputFileName(string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension, nameof(extension));

        string fileName;
        string filePath;

        do
        {
            Guid fileNameWithoutExtension = Guid.NewGuid();

            fileName = $"{fileNameWithoutExtension}.{extension}";
            filePath = GetOutputFilePath(fileName);
        }
        while (File.Exists(filePath));

        return fileName;
    }
}
