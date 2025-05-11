using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services.UI;

public interface IUiMessageService
{
    Task ShowAlertAsync(string message);
    Task ShowErrorAsync(HttpResponseMessage responseMessage);
}
