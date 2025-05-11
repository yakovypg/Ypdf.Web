using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface IBrowserFileSavingService
{
    Task<string> SaveAsync(IBrowserFile browserFile);
}
