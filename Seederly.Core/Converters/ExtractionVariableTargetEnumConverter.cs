using System.Text.Json;
using System.Text.Json.Serialization;
using Seederly.Core.Automation;

namespace Seederly.Core.Converters;

public class ExtractionVariableTargetEnumConverter : JsonConverter<ExtractionVariableTarget>
{
    public override ExtractionVariableTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Enum.TryParse<ExtractionVariableTarget>(value, ignoreCase: true, out var result))
            return result;

        throw new JsonException($"Unable to convert \"{value}\" to {nameof(ExtractionVariableTarget)}.");
    }

    public override void Write(Utf8JsonWriter writer, ExtractionVariableTarget value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}