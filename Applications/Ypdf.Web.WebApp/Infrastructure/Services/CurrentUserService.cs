using Ypdf.Web.Domain.Models.Api.Dto;
using Ypdf.Web.WebApp.Infrastructure.Utils;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class CurrentUserService : NotifiableObject, ICurrentUserService
{
    private UserDto? _user;

    public UserDto? User
    {
        get => _user;
        set => UpdateField(ref _user, value);
    }
}
