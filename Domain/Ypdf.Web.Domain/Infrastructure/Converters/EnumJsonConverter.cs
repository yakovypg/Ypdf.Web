using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ypdf.Web.Domain.Infrastructure.Converters;

public class EnumJsonConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert, nameof(typeToConvert));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        string value = reader.GetString() ?? string.Empty;
        return Enum.Parse<T>(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        string valueText = value.ToString();
        writer.WriteStringValue(valueText);
    }
}
