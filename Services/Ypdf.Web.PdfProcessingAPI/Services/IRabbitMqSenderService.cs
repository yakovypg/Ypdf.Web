using System.Threading.Tasks;

namespace Ypdf.Web.PdfProcessingAPI.Services;

public interface IRabbitMqSenderService
{
    Task SendMessageAsync(object obj);
    Task SendMessageAsync(string message);
}
