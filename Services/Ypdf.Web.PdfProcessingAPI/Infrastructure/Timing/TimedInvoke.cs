using System;
using System.Threading.Tasks;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Timing;

public static class TimedInvoke
{
    public static (DateTimeOffset Start, DateTimeOffset End) Invoke(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        DateTimeOffset start = DateTimeOffset.UtcNow;
        action.Invoke();
        DateTimeOffset end = DateTimeOffset.UtcNow;

        return (start, end);
    }

    public static async Task<(DateTimeOffset Start, DateTimeOffset End)> InvokeAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        DateTimeOffset start = DateTimeOffset.UtcNow;

        await action
            .Invoke()
            .ConfigureAwait(false);

        DateTimeOffset end = DateTimeOffset.UtcNow;

        return (start, end);
    }
}
