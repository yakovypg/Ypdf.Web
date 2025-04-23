using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Infrastructure.Services;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.AccoutAPI.Models.Dto;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.AccoutAPI.Commands;

public class RegisterUserCommand : BaseCommand, ICommand<RegisterUserRequest, RegisterUserResponse>
{
    private const SubscriptionType DefaultSubscriptionType = SubscriptionType.Trial;
    private static readonly TimeSpan DefaultSubscriptionPeriod = TimeSpan.FromDays(1);

    private readonly IUserRepository _userRepository;
    private readonly IEmailVerifierService _emailVerifierService;
    private readonly IPasswordVerifierService _passwordVerifierService;
    private readonly IUserNameVerifierService _userNameVerifierService;
    private readonly IUserSubscriptionService _userSubscriptionService;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IEmailVerifierService emailVerifierService,
        IPasswordVerifierService passwordVerifierService,
        IUserNameVerifierService userNameVerifierService,
        IUserSubscriptionService userSubscriptionService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(userRepository, nameof(userRepository));
        ArgumentNullException.ThrowIfNull(emailVerifierService, nameof(emailVerifierService));
        ArgumentNullException.ThrowIfNull(passwordVerifierService, nameof(passwordVerifierService));
        ArgumentNullException.ThrowIfNull(userNameVerifierService, nameof(userNameVerifierService));
        ArgumentNullException.ThrowIfNull(userSubscriptionService, nameof(userSubscriptionService));

        _userRepository = userRepository;
        _emailVerifierService = emailVerifierService;
        _passwordVerifierService = passwordVerifierService;
        _userNameVerifierService = userNameVerifierService;
        _userSubscriptionService = userSubscriptionService;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        SetDefaultUserNameIfNeeded(request);
        ValidateRequestParameters(request);

        Logger.LogInformation("Trying to register user with email {Email}", request.Email);

        await VerifyUserNotExistsAsync(request.Email!)
            .ConfigureAwait(false);

        User user = await _userRepository
            .AddAsync(request.Email!, request.Password!, request.UserName!)
            .ConfigureAwait(false);

        await _userSubscriptionService
            .AddSubscriptionAsync(user, DefaultSubscriptionType, DefaultSubscriptionPeriod)
            .ConfigureAwait(false);

        Logger.LogInformation("User with email {Email} registered", request.Email);

        UserDto userDto = Mapper.Map<UserDto>(user);
        RegisterUserResponse response = new(userDto);

        return response;
    }

    private void SetDefaultUserNameIfNeeded(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.UseDefaultUserName && string.IsNullOrEmpty(request.UserName))
        {
            request.UserName = request.Email;
            Logger.LogInformation("User name is set to {Email}", request.Email);
        }
    }

    private void ValidateRequestParameters(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (!_emailVerifierService.IsGood(request.Email))
        {
            Logger.LogWarning("Email {Email} is incorrect", request.Email);
            throw new BadRequestException("Email is incorrect");
        }

        if (!_passwordVerifierService.IsGood(request.Password))
        {
            Logger.LogWarning("Password is incorrect");
            throw new BadRequestException("Password is incorrect");
        }

        if (!_userNameVerifierService.IsGood(request.UserName))
        {
            Logger.LogWarning("User name {UserName} is incorrect", request.UserName);
            throw new BadRequestException("User name is incorrect");
        }
    }

    private async Task VerifyUserNotExistsAsync(string userEmail)
    {
        ArgumentNullException.ThrowIfNull(userEmail, nameof(userEmail));

        bool userExists = await _userRepository
            .ExistsAsync(userEmail)
            .ConfigureAwait(false);

        if (userExists)
        {
            Logger.LogWarning("User with email {Email} already exists", userEmail);
            throw new ConflictException($"User with email {userEmail} already exists");
        }
    }
}
