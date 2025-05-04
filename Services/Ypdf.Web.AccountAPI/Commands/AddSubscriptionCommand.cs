using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Infrastructure.Services;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api.Dto;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;

namespace Ypdf.Web.AccoutAPI.Commands;

public class AddSubscriptionCommand : BaseCommand, ICommand<AddSubscriptionRequest, AddSubscriptionResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSubscriptionService _userSubscriptionService;

    public AddSubscriptionCommand(
        IUserRepository userRepository,
        IUserSubscriptionService userSubscriptionService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(userRepository, nameof(userRepository));
        ArgumentNullException.ThrowIfNull(userSubscriptionService, nameof(userSubscriptionService));

        _userRepository = userRepository;
        _userSubscriptionService = userSubscriptionService;
    }

    public async Task<AddSubscriptionResponse> ExecuteAsync(AddSubscriptionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        ValidateRequestParameters(request);

        Logger.LogInformation(
            "Trying to add {SubscriptionType} subscription to {UserEmail}",
            request.SubscriptionType,
            request.UserEmail);

        User user = await FindUserAsync(request.UserEmail!)
            .ConfigureAwait(false);

        await _userSubscriptionService
            .AddSubscriptionAsync(user, request.SubscriptionType, request.Period)
            .ConfigureAwait(false);

        Logger.LogInformation(
            "Subscription {SubscriptionType} added to {UserEmail}",
            request.SubscriptionType,
            request.UserEmail);

        UserDto userDto = Mapper.Map<UserDto>(user);

        return new AddSubscriptionResponse(userDto);
    }

    private static void ValidateRequestParameters(AddSubscriptionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (string.IsNullOrEmpty(request.UserEmail))
            throw new BadRequestException("User email not specified");
    }

    private async Task<User> FindUserAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        User? user = await _userRepository
            .GetByEmailWithDependenciesAsync(email)
            .ConfigureAwait(false);

        if (user is null)
        {
            Logger.LogWarning("User with email {Email} not exists", email);
            throw new NotFoundException($"User with email {email} not found");
        }

        return user;
    }
}
