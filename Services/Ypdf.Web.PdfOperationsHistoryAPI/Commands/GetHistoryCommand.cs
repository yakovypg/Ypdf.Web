using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Commands;

public class GetHistoryCommand : BaseCommand, IProtectedCommand<GetHistoryRequest, GetHistoryResponse>
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
        ArgumentNullException.ThrowIfNull(
            pdfOperationResultRepository,
            nameof(pdfOperationResultRepository));

        _pdfOperationResultRepository = pdfOperationResultRepository;
    }

    public Task<GetHistoryResponse> ExecuteAsync(
        GetHistoryRequest request,
        ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        Logger.LogInformation("Trying to get history for user {UserId}", request.UserId);

        VerifyAccess(request.UserId, userClaims);

        IEnumerable<PdfOperationResult> results = _pdfOperationResultRepository
            .FindAll(t => t.UserId == request.UserId);

        Logger.LogInformation("History for user {UserId} successfully recieved", request.UserId);

        var response = new GetHistoryResponse(results);

        return Task.FromResult(response);
    }

    private void VerifyAccess(int pdfOperationResultUserId, ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        bool allowed = userClaims.VerifyAccess(
            ClaimTypes.NameIdentifier,
            pdfOperationResultUserId);

        if (!allowed)
        {
            Logger.LogWarning(
                "User {@User} doesn't have access to the resource owned by user {OwnerId}",
                userClaims.ToTypeValuePairs(),
                pdfOperationResultUserId);

            throw new ForbiddenException("User doesn't have access to the resource");
        }
    }
}
