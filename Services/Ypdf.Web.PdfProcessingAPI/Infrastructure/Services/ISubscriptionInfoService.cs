using System.Threading.Tasks;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public interface ISubscriptionInfoService
{
    Task<bool> IsOperationAllowedAsync(string subscriptionName, string operationName);
}
