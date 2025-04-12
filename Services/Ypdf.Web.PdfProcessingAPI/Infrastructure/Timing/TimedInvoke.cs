using System;
using System.Threading.Tasks;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;

public static class TimedInvoke
{
    public static (DateTime Start, DateTime End) Invoke(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        DateTime start = DateTime.Now;
        action.Invoke();
        DateTime end = DateTime.Now;

        return (start, end);
    }

    public static async Task<(DateTime Start, DateTime End)> InvokeAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        DateTime start = DateTime.Now;

        await action
            .Invoke()
            .ConfigureAwait(false);

        DateTime end = DateTime.Now;

        return (start, end);
    }
}
