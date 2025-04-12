using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public class GetOutputFileCommand : BaseCommand, ICommand<GetOutputFileRequest, GetOutputFileResponse>
{
    private readonly IConfiguration _configuration;

    public GetOutputFileCommand(
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

    public async Task<GetOutputFileResponse> ExecuteAsync(GetOutputFileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        using var memoryStream = new System.IO.MemoryStream();

        await System.Text.Json.JsonSerializer
            .SerializeAsync(memoryStream, request)
            .ConfigureAwait(false);

        string jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        Logger.LogInformation("GetOutputFile: {JsonData}", jsonData);

        string outputFilesDirectory = _configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");

        Logger.LogInformation("Directory: {OutputFilesDirectory}", outputFilesDirectory);

        return new GetOutputFileResponse();
    }
}
