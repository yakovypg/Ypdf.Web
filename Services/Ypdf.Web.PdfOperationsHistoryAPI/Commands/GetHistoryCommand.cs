using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Requests;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Responses;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Commands;

public class GetHistoryCommand : BaseCommand, ICommand<GetHistoryRequest, GetHistoryResponse>
{
    private readonly IPdfOperationResultRepository _pdfOperationResultRepository;

    public GetHistoryCommand(
        IPdfOperationResultRepository pdfOperationResultRepository,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(pdfOperationResultRepository, nameof(pdfOperationResultRepository));
        _pdfOperationResultRepository = pdfOperationResultRepository;
    }

    public Task<GetHistoryResponse> ExecuteAsync(GetHistoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        Logger.LogInformation("Trying to get history for user {UserId}", request.UserId);

        IEnumerable<PdfOperationResult> results = _pdfOperationResultRepository
            .FindAll(t => t.UserId == request.UserId);

        Logger.LogInformation("History for user {UserId} successfully recieved", request.UserId);

        var response = new GetHistoryResponse(results);

        return Task.FromResult(response);
    }
}
