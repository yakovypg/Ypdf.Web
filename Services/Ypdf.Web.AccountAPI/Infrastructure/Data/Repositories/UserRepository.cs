using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;
using Ypdf.Web.AccoutAPI.Infrastructure.Extensions;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.AccoutAPI.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AccountsDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        AccountsDbContext context,
        UserManager<User> userManager,
        ILogger<UserRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<User> AddAsync(string email, string password, string nickname)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(nickname, nameof(nickname));

        _logger.LogInformation("Trying to add user with email {Email}", email);

        var user = new User()
        {
            Email = email,
            UserName = email,
            Nickname = nickname
        };

        IdentityResult createUserResult = await _userManager
            .CreateAsync(user, password)
            .ConfigureAwait(false);

        if (!createUserResult.Succeeded)
        {
            _logger.LogWarning(
                "Cannot add user with email {Email}. Result: {@Result}",
                email,
                createUserResult);

            if (createUserResult.Errors.Any(t => t.DueToPassword()))
                throw new BadRequestException("Password is incorrect");

            throw new InternalException($"Cannot add user with email {email}");
        }

        _logger.LogInformation("User with email {Email} added", email);

        return user;
    }

    public async Task DeleteByEmailAsync(string email)
    {
        _logger.LogInformation("Trying to delete user with email {Email}", email);

        User? existingUser = await _userManager
            .FindByEmailAsync(email)
            .ConfigureAwait(false);

        if (existingUser is null)
        {
            _logger.LogInformation("User with email {Email} not found", email);
            throw new NotFoundException($"User with email {email} not found");
        }

        IdentityResult deleteUserResult = await _userManager
            .DeleteAsync(existingUser)
            .ConfigureAwait(false);

        if (!deleteUserResult.Succeeded)
        {
            _logger.LogWarning(
                "Cannot delete user with email {Email}. Result: {@Result}",
                email,
                deleteUserResult);

            throw new InternalException($"Cannot delete user with email {email}");
        }

        _logger.LogInformation("User with email {Email} deleted", email);
    }

    public async Task<User?> GetByEmailWithDependenciesAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

#nullable disable // EF makes everything null safe when accessing navigation
        return await _context.Users
            .Include(u => u.UserSubscription)
                .ThenInclude(us => us.Subscription)
            .SingleOrDefaultAsync(t => t.Email == email)
            .ConfigureAwait(false);
#nullable enable // EF makes everything null safe when accessing navigation
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        return await _userManager
            .FindByEmailAsync(email)
            .ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        User? existingUser = await GetByEmailAsync(email)
            .ConfigureAwait(false);

        return existingUser is not null;
    }
}
