using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.FilesAPI.Infrastructure.IO;

public class OutputFileCleaner : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    private readonly string _outputFilesDirectory;
    private readonly ILogger<OutputFileCleaner> _logger;

    public OutputFileCleaner(
        IConfiguration configuration,
        ILogger<OutputFileCleaner> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _outputFilesDirectory = configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");

        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            TryCleanOldFiles();

            await Task
                .Delay(CheckInterval, stoppingToken)
                .ConfigureAwait(false);
        }
    }

    private void TryCleanOldFiles()
    {
        try
        {
            CleanOldFiles();
            _logger.LogInformation("Old files cleaned");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to clean old files");
            _logger.LogError(ex, "{@Exception}", ex);
        }
    }

    private void CleanOldFiles()
    {
        var directoryInfo = new DirectoryInfo(_outputFilesDirectory);

        if (!directoryInfo.Exists)
        {
            _logger.LogWarning("Output files directory does not exist: {Directory}", _outputFilesDirectory);
            return;
        }

        DateTime currentTimeUtc = DateTime.UtcNow;
        DateTime minLastWriteTimeUtc = currentTimeUtc.AddDays(-1);

        FileInfo[] filesToDelete = directoryInfo
            .GetFiles()
            .Where(t => t.LastWriteTimeUtc < minLastWriteTimeUtc)
            .ToArray();

        _logger.LogInformation("Found {FilesCount} files to delete", filesToDelete.Length);

        DeleteFiles(filesToDelete);
    }

    private void DeleteFiles(IEnumerable<FileInfo> files)
    {
        ArgumentNullException.ThrowIfNull(files, nameof(files));

        foreach (FileInfo file in files)
        {
            try
            {
                file.Delete();
                _logger.LogInformation("File {FileFullName} deleted", file.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to delete file {FileFullName}", file.FullName);
                _logger.LogError(ex, "{@Exception}", ex);
            }
        }
    }
}
