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
	public class DataPointConverter<T> : JsonConverter
	{
		public static readonly DataPointConverter<T> Instance = new DataPointConverter<T>();

		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private DataPointConverter() { }

		public override bool CanConvert(Type objectType) => objectType == typeof(DataPoint<T>);

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

			return new DataPoint<T>
			(
				value: serializer.Deserialize<T>(reader),
				dateTime: DateTimeOffset.FromUnixTimeMilliseconds(serializer.Deserialize<long>(reader)).LocalDateTime
			);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var dataPoint = (DataPoint<T>)value;
			if (typeof(T) == typeof(CsvData))
			{
				CsvData csv = (CsvData)(object)dataPoint.Value;
				WriteDataPointArray(writer, serializer, dataPoint.EpochTime, csv.TotalNotional);
			}
		}
		public void WriteDataPointArray(JsonWriter writer,JsonSerializer serializer, long time, object dpValue)
		{
			writer.WriteStartArray();
			serializer.Serialize(writer, dpValue);
			serializer.Serialize(writer, time);
			writer.WriteEndArray();
		}
	}

}
