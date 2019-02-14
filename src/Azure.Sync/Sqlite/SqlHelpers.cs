using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Rocket.Surgery.Azure.Sync.SQLite
{
    internal class SqlHelpers
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Deserializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="storeType">Type of the store.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <returns></returns>
        public static JToken DeserializeValue(object value, string storeType, JTokenType columnType)
        {
            if (value == null)
            {
                return null;
            }

            if (IsTextType(storeType))
            {
                return ParseText(columnType, value);
            }
            if (IsRealType(storeType))
            {
                return ParseReal(columnType, value);
            }
            if (IsNumberType(storeType))
            {
                return ParseNumber(columnType, value);
            }

            return null;
        }

        /// <summary>
        /// Formats the member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public static string FormatMember(string memberName)
        {
            ValidateIdentifier(memberName);
            return $"[{memberName}]";
        }

        /// <summary>
        /// Formats the name of the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static string FormatTableName(string tableName)
        {
            ValidateIdentifier(tableName);
            return $"[{tableName}]";
        }

        // https://www.sqlite.org/datatype3.html (2.2 Affinity Name Examples)
        /// <summary>
        /// Gets the type of the store cast.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static string GetStoreCastType(Type type)
        {
            if (type == typeof(bool) ||
                type == typeof(DateTime) ||
                type == typeof(decimal))
            {
                return SqlColumnType.NUMERIC;
            }
            else if (type == typeof(int) ||
                     type == typeof(uint) ||
                     type == typeof(long) ||
                     type == typeof(ulong) ||
                     type == typeof(short) ||
                     type == typeof(ushort) ||
                     type == typeof(byte) ||
                     type == typeof(sbyte))
            {
                return SqlColumnType.INTEGER;
            }
            else if (type == typeof(float) ||
                     type == typeof(double))
            {
                return SqlColumnType.REAL;
            }
            else if (type == typeof(string) ||
                     type == typeof(Guid) ||
                     type == typeof(byte[]) ||
                     type == typeof(Uri) ||
                     type == typeof(TimeSpan))
            {
                return SqlColumnType.TEXT;
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Value of type '{0}' is not supported.", type.Name));
        }

        /// <summary>
        /// Gets the type of the store.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="allowNull">if set to <c>true</c> [allow null].</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static string GetStoreType(JTokenType type, bool allowNull)
        {
            switch (type)
            {
                case JTokenType.Boolean:
                    return SqlColumnType.BOOLEAN;

                case JTokenType.Integer:
                    return SqlColumnType.INTEGER;

                case JTokenType.Date:
                    return SqlColumnType.DATE_TIME;

                case JTokenType.Float:
                    return SqlColumnType.FLOAT;

                case JTokenType.String:
                    return SqlColumnType.TEXT;

                case JTokenType.Guid:
                    return SqlColumnType.GUID;

                case JTokenType.Array:
                case JTokenType.Object:
                    return SqlColumnType.JSON;

                case JTokenType.Bytes:
                    return SqlColumnType.BLOB;

                case JTokenType.Uri:
                    return SqlColumnType.URI;

                case JTokenType.TimeSpan:
                    return SqlColumnType.TIME_SPAN;

                case JTokenType.Null:
                    if (allowNull)
                    {
                        return null;
                    }
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Property of type '{0}' is not supported.", type));
                case JTokenType.Comment:
                case JTokenType.Constructor:
                case JTokenType.None:
                case JTokenType.Property:
                case JTokenType.Raw:
                case JTokenType.Undefined:
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Property of type '{0}' is not supported.", type));
            }
        }

        /// <summary>
        /// Serializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="allowNull">if set to <c>true</c> [allow null].</param>
        /// <returns></returns>
        public static object SerializeValue(JValue value, bool allowNull)
        {
            string storeType = GetStoreType(value.Type, allowNull);
            return SerializeValue(value, storeType, value.Type);
        }

        /// <summary>
        /// Serializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="storeType">Type of the store.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <returns></returns>
        public static object SerializeValue(JToken value, string storeType, JTokenType columnType)
        {
            if (value == null || value.Type == JTokenType.Null)
            {
                return null;
            }

            if (IsTextType(storeType))
            {
                return SerializeAsText(value, columnType);
            }
            if (IsRealType(storeType))
            {
                return SerializeAsReal(value, columnType);
            }
            if (IsNumberType(storeType))
            {
                return SerializeAsNumber(value, columnType);
            }

            return value.ToString();
        }

        private static JToken DeserializeDateTime(double value)
        {
            return Epoch.AddSeconds(value);
        }

        private static bool IsNumberType(string storeType)
        {
            return storeType == SqlColumnType.INTEGER ||
                   storeType == SqlColumnType.NUMERIC ||
                   storeType == SqlColumnType.BOOLEAN ||
                   storeType == SqlColumnType.DATE_TIME;
        }

        private static bool IsRealType(string storeType)
        {
            return storeType == SqlColumnType.REAL ||
                   storeType == SqlColumnType.FLOAT;
        }

        private static bool IsTextType(string storeType)
        {
            return storeType == SqlColumnType.TEXT ||
                   storeType == SqlColumnType.BLOB ||
                   storeType == SqlColumnType.GUID ||
                   storeType == SqlColumnType.JSON ||
                   storeType == SqlColumnType.URI ||
                   storeType == SqlColumnType.TIME_SPAN;
        }

        private static bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier) || identifier.Length > 128)
            {
                return false;
            }

            char first = identifier[0];
            if (!(char.IsLetter(first) || first == '_'))
            {
                return false;
            }

            for (int i = 1; i < identifier.Length; i++)
            {
                char ch = identifier[i];
                if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    return false;
                }
            }

            return true;
        }

        private static JToken ParseNumber(JTokenType type, object value)
        {
            if (type == JTokenType.Date)
            {
                return DeserializeDateTime(Convert.ToDouble(value));
            }

            long longValue = Convert.ToInt64(value);
            if (type == JTokenType.Boolean)
            {
                bool boolValue = longValue == 1;
                return boolValue;
            }
            return longValue;
        }

        private static JToken ParseReal(JTokenType type, object value)
        {
            double dblValue = Convert.ToDouble(value);
            if (type == JTokenType.Date) // for compatibility reason i.e. in earlier release datetime was serialized as real type
            {
                return DeserializeDateTime(dblValue);
            }

            return dblValue;
        }

        private static JToken ParseText(JTokenType type, object value)
        {
            string strValue = value as string;
            if (value == null)
            {
                return strValue;
            }

            if (type == JTokenType.Guid)
            {
                return Guid.Parse(strValue);
            }
            if (type == JTokenType.Bytes)
            {
                return Convert.FromBase64String(strValue);
            }
            if (type == JTokenType.TimeSpan)
            {
                return TimeSpan.Parse(strValue);
            }
            if (type == JTokenType.Uri)
            {
                return new Uri(strValue, UriKind.RelativeOrAbsolute);
            }
            if (type == JTokenType.Array || type == JTokenType.Object)
            {
                return JToken.Parse(strValue);
            }

            return strValue;
        }

        private static object SerializeAsNumber(JToken value, JTokenType columnType)
        {
            if (columnType == JTokenType.Date)
            {
                return SerializeDateTime(value);
            }
            return value.Value<long>();
        }

        private static double SerializeAsReal(JToken value, JTokenType columnType)
        {
            return value.Value<double>();
        }

        private static string SerializeAsText(JToken value, JTokenType columnType)
        {
            if (columnType == JTokenType.Bytes && value.Type == JTokenType.Bytes)
            {
                return Convert.ToBase64String(value.Value<byte[]>());
            }

            return value.ToString();
        }

        private static long SerializeDateTime(JToken value)
        {
            //var date = value.ToObject<DateTime>();
            //if (date.Kind == DateTimeKind.Unspecified)
            //{
            //	date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            //}
            //double unixTimestamp = Math.Round((date.ToUniversalTime() - epoch).TotalSeconds, 3);
            //return unixTimestamp;

            var date = value.ToObject<DateTimeOffset>();

            long unixTimestamp = date.Ticks;
            return unixTimestamp;
        }

        private static void ValidateIdentifier(string identifier)
        {
            if (!IsValidIdentifier(identifier))
            {
                throw new ArgumentException(
                    $"'{identifier}' is not a valid identifier. Identifiers must be under 128 characters in length, start with a letter or underscore, and can contain only alpha-numeric and underscore characters.", nameof(identifier));
            }
        }
    }
}