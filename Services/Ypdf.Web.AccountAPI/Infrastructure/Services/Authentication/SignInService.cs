using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;

public class SignInService : ISignInService
{
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<SignInService> _logger;

    public SignInService(
        IUserRepository userRepository,
        SignInManager<User> signInManager,
        ILogger<SignInService> logger)
    {
        ArgumentNullException.ThrowIfNull(userRepository, nameof(userRepository));
        ArgumentNullException.ThrowIfNull(signInManager, nameof(signInManager));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _userRepository = userRepository;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<User> SignInAsync(string email, string password)
    {
        _logger.LogInformation("Trying to authenticate user with email {Email}", email);

        User? user = await _userRepository
            .GetByEmailWithDependenciesAsync(email)
            .ConfigureAwait(false);

        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not exists", email);
            throw new BadRequestException($"User email {email} is invalid");
        }

        SignInResult signInResult = await _signInManager
            .PasswordSignInAsync(user, password, false, false)
            .ConfigureAwait(false);

        if (!signInResult.Succeeded)
        {
            _logger.LogWarning(
                "User with email {Email} isn't authenticated. Result: {@Result}",
                email,
                signInResult);

            throw new BadRequestException($"Invalid email or password");
        }

        _logger.LogInformation("User with email {Email} authenticated successfully", email);

        return user;
    }
}
