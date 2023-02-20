using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StackBattleConsoleGameServer;

internal class ArmyConverter : JsonConverter<Army>
{
    public override Army Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        reader.Read();
        CheckPropertyName("TeamName", ref reader);

        reader.Read();
        var teamName = reader.GetString() ?? $"DefaultTeamName{Guid.NewGuid()}";

        var stringConverter = (JsonConverter<string>)options.GetConverter(typeof(string));

        var unitDescriptions = new Dictionary<int, UnitDescription>();

        var unitDescriptionsReaded = DeserializeEnumerable<UnitDescription>(
            "UnitDescriptions",
            ref reader,
            options,
            (JsonConverter<UnitDescription> converter, ref Utf8JsonReader reader, JsonSerializerOptions options) =>
                converter.Read(ref reader, typeof(UnitDescription), options)!);

        foreach (var unitDescription in unitDescriptionsReaded)
            unitDescriptions.Add(unitDescription.UnidDescriptionId, unitDescription);

        var unitsReaded = DeserializeEnumerable<int>(
            "Units",
            ref reader,
            options,
            (JsonConverter<int> converter, ref Utf8JsonReader reader, JsonSerializerOptions options) =>
                converter.Read(ref reader, typeof(int), options));

        var units = new List<IUnit>();
        foreach (var unit in unitsReaded)
            units.Add(new Unit(unitDescriptions[unit]));

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException();

        return new Army { TeamName = teamName, UnitDescriptions = unitDescriptions, Units = units };
    }

    private delegate T DeserializeDelegate<T>(JsonConverter<T> converter, ref Utf8JsonReader reader, JsonSerializerOptions options);

    private static IEnumerable<T> DeserializeEnumerable<T>(
        string propertyName,
        ref Utf8JsonReader reader,
        JsonSerializerOptions options,
        DeserializeDelegate<T> action)
    {
        reader.Read();
        CheckPropertyName(propertyName, ref reader);

        reader.Read();
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var list = new List<T>();

        var converter = (JsonConverter<T>)options.GetConverter(typeof(T));

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return list;

            T? value = action(converter, ref reader, options);

            if (value == null)
                throw new JsonException();

            list.Add(value);
        }

        throw new JsonException();
    }

    private static void CheckPropertyName(string propertyName, ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();

        string? propertyNameReaded = reader.GetString();

        // For performance, parse with ignoreCase:false first.
        if (!propertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
            throw new JsonException($"Unable to get right property name. Waiting for '{propertyName}, but get '{propertyNameReaded}'.");
    }

    public override void Write(Utf8JsonWriter writer, Army army, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("TeamName", army.TeamName);

        SerializeToArray("UnitDescriptions", army.UnitDescriptions.Values, writer,
            options, (converter, writer, value, options) => converter.Write(writer, value, options));

        SerializeToArray("Units", army.Units, writer,
            options, (converter, writer, value, options) =>
                writer.WriteNumberValue(value.UnitDescription.UnidDescriptionId));

        writer.WriteEndObject();
    }

    private static void SerializeToArray<T>(
        string propertyName,
        IEnumerable<T> values,
        Utf8JsonWriter writer,
        JsonSerializerOptions options,
        Action<JsonConverter<T>, Utf8JsonWriter, T, JsonSerializerOptions> action)
    {
        var converter = (JsonConverter<T>)options.GetConverter(typeof(T));

        writer.WriteStartArray(propertyName);

        foreach (var value in values)
            action(converter, writer, value, options);

        writer.WriteEndArray();
    }
}
