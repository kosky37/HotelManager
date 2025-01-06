using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelManager.Common;

namespace HotelManager.Data.JsonConverters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (DateOnly.TryParseExact(dateString, Consts.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            return dateOnly;
        }

        throw new JsonException($"Unable to parse '{dateString}' as DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Consts.DateFormat, CultureInfo.InvariantCulture));
    }
}