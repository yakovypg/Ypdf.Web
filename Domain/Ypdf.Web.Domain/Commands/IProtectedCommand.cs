using System.Security.Claims;
using System.Threading.Tasks;

namespace Ypdf.Web.Domain.Commands;

#pragma warning disable SA1402 // File may only contain a single type

public interface IProtectedCommand<TIn, TOut>
{
    Task<TOut> ExecuteAsync(TIn request, ClaimsPrincipal userClaims);
}

public interface IProtectedCommand<TOut>
{
    Task<TOut> ExecuteAsync(ClaimsPrincipal userClaims);
}

#pragma warning restore SA1402 // File may only contain a single type
