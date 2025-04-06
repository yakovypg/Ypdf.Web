using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.AccoutAPI.Commands;

public class MergeCommand : BaseCommand, ICommand<MergeRequest, PdfOperationResponse>
{
    private readonly IConfiguration _configuration;

    public MergeCommand(
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _configuration = configuration;
    }

    public async Task<PdfOperationResponse> ExecuteAsync(MergeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        using var memoryStream = new System.IO.MemoryStream();

        await System.Text.Json.JsonSerializer
            .SerializeAsync(memoryStream, request)
            .ConfigureAwait(false);

        string jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        Logger.LogInformation("MERGE: {JsonData}", jsonData);

        string outputFilesDirectory = _configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");

        Logger.LogInformation("Directory: {OutputFilesDirectory}", outputFilesDirectory);

        await System.IO.File
            .WriteAllTextAsync($"{DateTime.Now.Ticks}", string.Empty)
            .ConfigureAwait(false);

        return new PdfOperationResponse();
    }
}
