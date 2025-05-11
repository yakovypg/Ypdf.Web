using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Api;

public interface IApiResponseReaderService
{
    Task<T?> ReadAsync<T>(HttpResponseMessage responseMessage);
    Task<(bool Success, T? Response)> ReadWithInfoAsync<T>(HttpResponseMessage responseMessage);
}
