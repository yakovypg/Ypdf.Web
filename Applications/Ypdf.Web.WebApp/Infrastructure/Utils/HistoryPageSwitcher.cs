using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.WebApp.Infrastructure.Services.Api;
using Ypdf.Web.WebApp.Infrastructure.Services.Http;
using Ypdf.Web.WebApp.Infrastructure.Services.UI;

namespace Ypdf.Web.WebApp.Infrastructure.Utils;

public class HistoryPageSwitcher
{
    private const int StartPage = 1;
    private const int DefaultRecordsPerPage = 9;

    private readonly IUiMessageService _messageService;
    private readonly IJsElementInteractorService _elementInteractorService;
    private readonly IApiResponseReaderService _apiResponseReaderService;
    private readonly IHttpClientInteractorService _httpClientInteractorService;
    private readonly IEndpointUrlsService _endpointUrlsService;

    private string _userId;
    private int _minHistoryPageNumber = 1;
    private int _maxHistoryPageNumber = 1;

    public HistoryPageSwitcher(
        IUiMessageService messageService,
        IJsElementInteractorService elementInteractor,
        IApiResponseReaderService apiResponseReader,
        IHttpClientInteractorService httpClientInteractor,
        IEndpointUrlsService endpointUrlsService,
        int recordsPerPage = DefaultRecordsPerPage)
    {
        ArgumentNullException.ThrowIfNull(messageService, nameof(messageService));
        ArgumentNullException.ThrowIfNull(elementInteractor, nameof(elementInteractor));
        ArgumentNullException.ThrowIfNull(apiResponseReader, nameof(apiResponseReader));
        ArgumentNullException.ThrowIfNull(httpClientInteractor, nameof(httpClientInteractor));
        ArgumentNullException.ThrowIfNull(endpointUrlsService, nameof(endpointUrlsService));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recordsPerPage, nameof(recordsPerPage));

        _userId = string.Empty;

        _messageService = messageService;
        _elementInteractorService = elementInteractor;
        _apiResponseReaderService = apiResponseReader;
        _httpClientInteractorService = httpClientInteractor;
        _endpointUrlsService = endpointUrlsService;

        RecordsPerPage = recordsPerPage;
    }

    public int RecordsPerPage { get; }
    public int CurrentPageNumber { get; private set; }

    public string UserId
    {
        get => _userId;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);

            if (value == _userId)
                return;

            _userId = value;
            _minHistoryPageNumber = StartPage;
            _maxHistoryPageNumber = StartPage;

            CurrentPageNumber = StartPage;
        }
    }

    public async Task PreviousPageAsync()
    {
        VerifyUserIdIsSpecidied();

        if (CurrentPageNumber <= _minHistoryPageNumber)
            return;

        await ChangePageAsync(--CurrentPageNumber)
            .ConfigureAwait(false);
    }

    public async Task NextPageAsync()
    {
        VerifyUserIdIsSpecidied();

        if (CurrentPageNumber >= _maxHistoryPageNumber)
            return;

        await ChangePageAsync(++CurrentPageNumber)
            .ConfigureAwait(false);
    }

    public async Task ChangePageAsync(int newPageNumber)
    {
        if (newPageNumber < _minHistoryPageNumber)
            newPageNumber = _minHistoryPageNumber;

        if (newPageNumber > _maxHistoryPageNumber)
            newPageNumber = _maxHistoryPageNumber;

        CurrentPageNumber = newPageNumber;

        await DisplayPageAsync(newPageNumber)
            .ConfigureAwait(false);

        bool previousButtonDisabled = newPageNumber == _minHistoryPageNumber;
        bool nextButtonDisabled = newPageNumber == _maxHistoryPageNumber;

        await _elementInteractorService
            .SetDisabledAsync("prev-button", previousButtonDisabled)
            .ConfigureAwait(false);

        await _elementInteractorService
            .SetDisabledAsync("next-button", nextButtonDisabled)
            .ConfigureAwait(false);
    }

    private static string GenerateTableRowHtml(PdfOperationResult result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));

        DateTimeOffset endDateLocal = result.EndDate.ToLocalTime();

        string operationName = result.OperationType.ToString();
        string operationEndDate = endDateLocal.Date.ToShortDateString();
        string outputFileName = result.OutputFileName;
        string outputFileUrl = $"/result/{outputFileName}";

        DateTime currentTimeUtc = DateTime.UtcNow;
        DateTime minLastWriteTimeUtc = currentTimeUtc.AddDays(-1);

        string resultLinkHtml = result.EndDate >= minLastWriteTimeUtc
            ? $"<a class='link-offset-1' href='{outputFileUrl}'>{outputFileName}</a>"
            : "<span>outdate</span>";

        return $@"
            <tr>
                <td>{operationEndDate}</td>
                <td>{operationName}</td>
                <td>{resultLinkHtml}</td>
            </tr>
        ";
    }

    private void VerifyUserIdIsSpecidied()
    {
        if (string.IsNullOrEmpty(UserId))
            throw new InvalidOperationException($"{nameof(UserId)} not initialized");
    }

    private async Task DisplayPageAsync(int pageNumber)
    {
        async void SuccessHandler(HttpResponseMessage responseMessage)
        {
            (bool success, GetHistoryResponse? response) = await _apiResponseReaderService
                .ReadWithInfoAsync<GetHistoryResponse>(responseMessage)
                .ConfigureAwait(false);

            if (!success || response is null)
            {
                await _messageService
                    .ShowAlertAsync("Failed to display history page")
                    .ConfigureAwait(false);

                return;
            }

            _minHistoryPageNumber = response.MinPage;
            _maxHistoryPageNumber = response.MaxPage;

            await DisplayHistoryAsync(response.PdfOperationResults)
                .ConfigureAwait(false);
        }

        Uri uri = _endpointUrlsService.History(UserId, pageNumber, RecordsPerPage);

        await _httpClientInteractorService
            .GetAsync(uri, SuccessHandler)
            .ConfigureAwait(false);
    }

    private async Task DisplayHistoryAsync(IEnumerable<PdfOperationResult> history)
    {
        ArgumentNullException.ThrowIfNull(history, nameof(history));

        await _elementInteractorService
            .SetInnerHtmlAsync("#operationTable tbody", string.Empty)
            .ConfigureAwait(false);

        foreach (PdfOperationResult result in history)
        {
            string rowHtml = GenerateTableRowHtml(result);

            await _elementInteractorService
                .InsertAdjacentHtmlAsync("#operationTable tbody", rowHtml)
                .ConfigureAwait(false);
        }
    }
}
