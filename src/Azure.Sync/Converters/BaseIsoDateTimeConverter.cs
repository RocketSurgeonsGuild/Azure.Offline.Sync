using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rocket.Surgery.Azure.Sync
{
    public class BaseIsoDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Converts DateTime and DateTimeOffset object into UTC DateTime.
        /// </summary>
        public BaseIsoDateTimeConverter()
        {
            Culture = CultureInfo.InvariantCulture;
            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffk";
        }

        /// <inheritdoc />
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object datetimeObject = base.ReadJson(reader, objectType, existingValue, serializer);

            if (datetimeObject == null)
            {
                return null;
            }

            switch (datetimeObject)
            {
                case DateTime time:
                    return time;

                case DateTimeOffset timeoffset:
                    return timeoffset;
            }

            return datetimeObject;
        }

        /// <inheritdoc />
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTimeOffset dateTime;
            if (value is DateTime time)
            {
                dateTime = time.ToUniversalTime();
            }
            else
            {
                dateTime = (DateTimeOffset)value;
            }

            base.WriteJson(writer, dateTime, serializer);
        }
    }
}