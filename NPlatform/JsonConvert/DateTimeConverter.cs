﻿using System.Text.Json.Serialization;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string Format;
    public DateTimeConverter(string format)
    {
        Format = format;
    }
    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(Format));
    }
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), Format, null);
    }

}