using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.PdfProcessingAPI.Models;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class SubscriptionInfoService : ISubscriptionInfoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SubscriptionInfoService> _logger;

    public SubscriptionInfoService(IConfiguration configuration, ILogger<SubscriptionInfoService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> IsOperationAllowedAsync(string subscriptionName, string operationName)
    {
        ArgumentNullException.ThrowIfNull(subscriptionName, nameof(subscriptionName));
        ArgumentNullException.ThrowIfNull(operationName, nameof(operationName));

        _logger.LogInformation(
            "Checking if operation {Operation} is allowed for subscription {Subscription}",
            operationName,
            subscriptionName);

        SubscriptionAllowedOperations? allowedOperations = await
            GetSubscriptionAllowedOperations(subscriptionName)
            .ConfigureAwait(false);

        if (allowedOperations is null)
        {
            _logger.LogWarning(
                "Allowed operations for subscription {SubscriptionName} not found",
                subscriptionName);

            return false;
        }

        bool operationAllowed = allowedOperations.AllowedOperations.Contains(operationName);
        LogAvailabilityOfOperation(operationName, subscriptionName, operationAllowed);

        return operationAllowed;
    }

    private async Task<SubscriptionAllowedOperations?> GetSubscriptionAllowedOperations(
        string subscriptionName)
    {
        ArgumentNullException.ThrowIfNull(subscriptionName, nameof(subscriptionName));

        string allowedOperationsFilePath = _configuration["Resources:SubscriptionAllowedOperations"]
            ?? throw new ConfigurationException("Allowed operations file path not specified");

        string allowedOperationsJson = await File
            .ReadAllTextAsync(allowedOperationsFilePath)
            .ConfigureAwait(false);

        IEnumerable<SubscriptionAllowedOperations>? allowedOperations = JsonSerializer
            .Deserialize<IEnumerable<SubscriptionAllowedOperations>>(allowedOperationsJson);

        if (allowedOperations is null)
            throw new InternalException($"Cannot deserialize data from {allowedOperationsFilePath}");

        return allowedOperations.FirstOrDefault(t => t.SubscriptionName == subscriptionName);
    }

    private void LogAvailabilityOfOperation(
        string operationName,
        string subscriptionName,
        bool operationAllowed)
    {
        ArgumentNullException.ThrowIfNull(subscriptionName, nameof(subscriptionName));
        ArgumentNullException.ThrowIfNull(operationName, nameof(operationName));

        string notItem = operationAllowed ? string.Empty : "n't";

        _logger.LogInformation(
                "Operation {Operation} is{NotItem} allowed for subscription {Subscription}",
                operationName,
                notItem,
                subscriptionName);
    }
}
