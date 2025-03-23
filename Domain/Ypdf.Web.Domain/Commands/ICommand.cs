using System.Threading.Tasks;

namespace Ypdf.Web.Domain.Commands;

#pragma warning disable SA1402 // File may only contain a single type

public interface ICommand<TIn, TOut>
{
    Task<TOut> ExecuteAsync(TIn request);
}

public interface ICommand<TOut>
{
    Task<TOut> ExecuteAsync();
}

#pragma warning restore SA1402 // File may only contain a single type
