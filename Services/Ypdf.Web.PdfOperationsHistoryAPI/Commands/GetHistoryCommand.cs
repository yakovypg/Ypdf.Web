using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Dto.Requests;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Dto.Responses;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Commands;

public class GetHistoryCommand : BaseCommand, ICommand<GetHistoryRequest, GetHistoryResponse>
{
    public GetHistoryCommand(
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    public async Task<GetHistoryResponse> ExecuteAsync(GetHistoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        string jsonData = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation("GetHistory: {JsonData}", jsonData);

        return new GetHistoryResponse();
    }
}
