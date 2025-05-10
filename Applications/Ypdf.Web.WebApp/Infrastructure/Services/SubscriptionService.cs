using System;
using System.Threading.Tasks;
using Ypdf.Web.Domain.Models;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUiMessageService _messageService;
    private readonly ICurrentUserService _currentUserService;

    public SubscriptionService(
        IUiMessageService messageService,
        ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(messageService, nameof(messageService));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        _messageService = messageService;
        _currentUserService = currentUserService;
    }

    public async Task ActivateAsync(SubscriptionType subscriptionType)
    {
        await _messageService
            .ShowAlertAsync("Not supported yet")
            .ConfigureAwait(false);
    }
}
