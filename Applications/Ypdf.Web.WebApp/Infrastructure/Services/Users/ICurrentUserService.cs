using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Users;

public interface ICurrentUserService
{
    UserDto? User { get; set; }
}
