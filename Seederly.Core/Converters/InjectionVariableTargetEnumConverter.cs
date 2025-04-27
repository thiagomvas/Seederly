using System.Text.Json;
using System.Text.Json.Serialization;
using Seederly.Core.Automation;

namespace Seederly.Core.Converters;

public class InjectionVariableTargetEnumConverter : JsonConverter<InjectionVariableTarget>
{
    public override InjectionVariableTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Enum.TryParse<InjectionVariableTarget>(value, ignoreCase: true, out var result))
            return result;

        throw new JsonException($"Unable to convert \"{value}\" to {nameof(InjectionVariableTarget)}.");
    }

    public override void Write(Utf8JsonWriter writer, InjectionVariableTarget value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}