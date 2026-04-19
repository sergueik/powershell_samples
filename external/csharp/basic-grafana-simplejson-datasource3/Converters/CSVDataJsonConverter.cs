using GrafanaGenericSimpleJsonDataSource.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Converters
{
    public class CSVDataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CsvData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token {0} when parsing data point.", reader.TokenType));
            }

            if (!reader.Read())
            {
                throw new JsonSerializationException("Unexpected end when reading JSON.");
            }

            return new CsvData
            (
                serializer.Deserialize<string>(reader),
                DateTimeOffset.FromUnixTimeMilliseconds(serializer.Deserialize<long>(reader)).LocalDateTime,
                serializer.Deserialize<int>(reader),
                serializer.Deserialize<int>(reader)
            );
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataPoint = (CsvData)value;

            writer.WriteStartArray();
            serializer.Serialize(writer, dataPoint.Name);
            serializer.Serialize(writer, dataPoint.Date);
            serializer.Serialize(writer, dataPoint.TotalFilled);
            serializer.Serialize(writer, dataPoint.TotalNotional);
            writer.WriteEndArray();
        }
    }
}
