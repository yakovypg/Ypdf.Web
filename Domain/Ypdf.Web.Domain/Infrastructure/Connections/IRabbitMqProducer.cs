using System.Threading.Tasks;

namespace Ypdf.Web.Domain.Infrastructure.Connections;

public interface IRabbitMqProducer
{
    Task SendMessageAsync(object obj);
    Task SendMessageAsync(string message);
}
