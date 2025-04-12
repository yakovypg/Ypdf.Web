using System.Threading.Tasks;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface IRabbitMqProducerService
{
    Task SendMessageAsync(object obj);
    Task SendMessageAsync(string message);
}
