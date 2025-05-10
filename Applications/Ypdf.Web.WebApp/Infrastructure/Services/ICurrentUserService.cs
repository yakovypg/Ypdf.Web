using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface ICurrentUserService
{
    UserDto? User { get; set; }
}
