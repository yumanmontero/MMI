using MosaMosaicIntegration.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MosaMosaicIntegration.Controlador
{
    public class MicrosecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long num;
            if (reader.Value == null) { return null; }
            string valo = reader.Value.ToString();
            if (long.TryParse(valo, out num))
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(ApplicationConstants.timeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(_epoch.AddMilliseconds((long)reader.Value),tz);
            }
            else
            {
                DateTime time = DateTime.ParseExact(valo, ApplicationConstants.dfus, System.Globalization.CultureInfo.InvariantCulture);
                return time;
            }

            
        }
    }
}
