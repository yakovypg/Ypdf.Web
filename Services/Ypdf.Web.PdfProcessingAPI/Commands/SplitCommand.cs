using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;

namespace Ypdf.Web.AccoutAPI.Commands;

public class SplitCommand : BaseCommand, ICommand<SplitRequest, PdfOperationResponse>
{
    public SplitCommand(IMapper mapper, ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    public async Task<PdfOperationResponse> ExecuteAsync(SplitRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        using var memoryStream = new System.IO.MemoryStream();

        await System.Text.Json.JsonSerializer
            .SerializeAsync(memoryStream, request)
            .ConfigureAwait(false);

        string jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        Logger.LogInformation("SPLIT: {JsonData}", jsonData);

        return new PdfOperationResponse();
    }
}
