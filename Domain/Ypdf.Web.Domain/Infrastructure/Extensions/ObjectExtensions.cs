using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ypdf.Web.Domain.Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static async Task<string> ToJsonAsync(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        using var memoryStream = new System.IO.MemoryStream();

        await JsonSerializer
            .SerializeAsync(memoryStream, obj)
            .ConfigureAwait(false);

        byte[] jsonDataBytes = memoryStream.ToArray();
        string jsonData = Encoding.UTF8.GetString(jsonDataBytes);

        return jsonData;
    }
}
