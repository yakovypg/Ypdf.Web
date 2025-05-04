using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface IJsElementInteractorService
{
    Task SetDisabledAsync(string itemId, bool disabled);
    Task SetInnerHtmlAsync(string selector, string html);
    Task InsertAdjacentHtmlAsync(string selector, string html);
}
