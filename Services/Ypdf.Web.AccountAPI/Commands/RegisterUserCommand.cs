using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Infrastructure.Services;
using Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.AccoutAPI.Models.Dto;
using Ypdf.Web.AccoutAPI.Models.Requests;
using Ypdf.Web.AccoutAPI.Models.Responses;
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
    private readonly INicknameVerifierService _nicknameVerifierService;
    private readonly IUserSubscriptionService _userSubscriptionService;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IEmailVerifierService emailVerifierService,
        IPasswordVerifierService passwordVerifierService,
        INicknameVerifierService nicknameVerifierService,
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
        ArgumentNullException.ThrowIfNull(nicknameVerifierService, nameof(nicknameVerifierService));
        ArgumentNullException.ThrowIfNull(userSubscriptionService, nameof(userSubscriptionService));

        _userRepository = userRepository;
        _emailVerifierService = emailVerifierService;
        _passwordVerifierService = passwordVerifierService;
        _nicknameVerifierService = nicknameVerifierService;
        _userSubscriptionService = userSubscriptionService;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        SetDefaultNicknameIfNeeded(request);
        ValidateRequestParameters(request);

        Logger.LogInformation("Trying to register user with email {Email}", request.Email);

        await VerifyUserNotExistsAsync(request.Email!)
            .ConfigureAwait(false);

        User user = await _userRepository
            .AddAsync(request.Email!, request.Password!, request.Nickname!)
            .ConfigureAwait(false);

        await _userSubscriptionService
            .AddSubscriptionAsync(user, DefaultSubscriptionType, DefaultSubscriptionPeriod)
            .ConfigureAwait(false);

        Logger.LogInformation("User with email {Email} registered", request.Email);

        UserDto userDto = Mapper.Map<UserDto>(user);
        RegisterUserResponse response = new(userDto);

        return response;
    }

    private void SetDefaultNicknameIfNeeded(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.UseDefaultNickname && string.IsNullOrEmpty(request.Nickname))
        {
            request.Nickname = request.Email;
            Logger.LogInformation("Nickname is set to {Email}", request.Email);
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

        if (!_nicknameVerifierService.IsGood(request.Nickname))
        {
            Logger.LogWarning("Nickname {Nickname} is incorrect", request.Nickname);
            throw new BadRequestException("Nickname is incorrect");
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
