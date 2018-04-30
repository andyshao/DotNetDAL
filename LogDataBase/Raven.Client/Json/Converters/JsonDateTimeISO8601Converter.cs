using System;
using System.Globalization;
using Newtonsoft.Json;
using Sparrow;
using Sparrow.Extensions;

namespace Raven.Client.Json.Converters
{
    internal sealed class JsonDateTimeISO8601Converter : RavenJsonConverter
    {
        public static readonly JsonDateTimeISO8601Converter Instance = new JsonDateTimeISO8601Converter();

        private JsonDateTimeISO8601Converter()
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                if (dateTime.Kind == DateTimeKind.Unspecified)
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                writer.WriteValue(dateTime.GetDefaultRavenFormat(dateTime.Kind == DateTimeKind.Utc));
            }
            else if (value is DateTimeOffset)
            {
                var dateTimeOffset = (DateTimeOffset)value;
                //DefaultFormat.DateTimeOffsetFormatsToWrite
                writer.WriteValue(dateTimeOffset.ToString("o", CultureInfo.InvariantCulture));
            }
            else
                throw new ArgumentException(string.Format("Not idea how to process argument: '{0}'", value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string DateTimeOffsetFormatsToWrite = "o";
            string DateTimeFormatsToWrite = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff";

            string[] DateTimeFormatsToRead = {
            DateTimeOffsetFormatsToWrite,
            DateTimeFormatsToWrite,
            "yyyy-MM-ddTHH:mm:ss.fffffffzzz",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
            "r",
            "yyyy-MM-ddTHH:mm:ss.fffK",
            "yyyy-MM-ddTHH:mm:ss.FFFK",
        };

            var s = reader.Value as string;
            if (s != null)
            {
                if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
                {
                    DateTime time;
                    if (DateTime.TryParseExact(s, DateTimeFormatsToRead, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out time))
                    {
                        if (s.EndsWith("+00:00"))
                            return time.ToUniversalTime();
                        return time;
                    }
                }
                if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
                {
                    DateTimeOffset time;
                    if (DateTimeOffset.TryParseExact(s, DateTimeFormatsToRead, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out time))
                        return time;
                }

            }
            return DeferReadToNextConverter(reader, objectType, serializer, existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType ||
                typeof(DateTimeOffset) == objectType ||
                typeof(DateTimeOffset?) == objectType ||
                typeof(DateTime?) == objectType;
        }
    }
}
