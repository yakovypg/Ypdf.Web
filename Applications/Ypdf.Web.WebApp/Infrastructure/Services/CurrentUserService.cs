using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public UserDto? User { get; set; }
}
