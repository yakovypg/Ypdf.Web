using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface IApiResponseReaderService
{
    Task<T?> ReadAsync<T>(HttpResponseMessage responseMessage);

    Task<(bool Success, T? Response)> TryReadAsync<T>(HttpResponseMessage responseMessage);
}
