using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Dapper
{
    /// <summary>
    /// TypeMapper Interface
    /// </summary>
    public interface IEntityMapper
    {
        MemberInfo FindMember(MemberInfo[] properties, DbDataInfo dataInfo);
        MethodInfo FindConvertMethod(Type csharpType, Type dbType);
        DbDataInfo FindConstructorParameter(DbDataInfo[] dataInfos, ParameterInfo parameterInfo);
        ConstructorInfo FindConstructor(Type csharpType);
    }

    /// <summary>
    /// 返回数据记录到Csharp类型的策略
    /// </summary>
    public class EntityMapper : IEntityMapper
    {
        public bool MatchNamesWithUnderscores { get; set; }

        /// <summary>
        /// Find parametric constructors.
        /// If there is no default constructor, the constructor with the most parameters is returned.
        /// </summary>
        public ConstructorInfo FindConstructor(Type csharpType)
        {
            var constructor = csharpType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                var constructors = csharpType.GetConstructors();
                constructor = constructors.Where(a => a.GetParameters().Length == constructors.Max(s => s.GetParameters().Length)).FirstOrDefault();
            }
            return constructor;
        }

        /// <summary>
        /// Returns field information based on parameter information
        /// </summary>
        public DbDataInfo FindConstructorParameter(DbDataInfo[] dataInfos, ParameterInfo parameterInfo)
        {
            foreach (var item in dataInfos)
            {
                if (item.DataName.Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
                else if (MatchNamesWithUnderscores && item.DataName.Replace("_", "").Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns attribute information based on field information
        /// </summary>
        public MemberInfo FindMember(MemberInfo[] properties, DbDataInfo dataInfo)
        {
            foreach (var item in properties)
            {
                if (item.Name.Equals(dataInfo.DataName, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
                else if (MatchNamesWithUnderscores && item.Name.Equals(dataInfo.DataName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Return type conversion function.
        /// </summary>
        public MethodInfo FindConvertMethod(Type csharpType, Type dbType)
        {
            if (GetUnderlyingType(dbType) == typeof(bool) || GetUnderlyingType(csharpType) == typeof(bool))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToBooleanMethod : DataConvertMethod.ToBooleanNullableMethod;
            }
            if (GetUnderlyingType(csharpType).IsEnum)
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToEnumMethod.MakeGenericMethod(csharpType) : DataConvertMethod.ToEnumNullableMethod.MakeGenericMethod(GetUnderlyingType(csharpType));
            }
            if (GetUnderlyingType(dbType) == typeof(char) || GetUnderlyingType(csharpType) == typeof(char))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToCharMethod : DataConvertMethod.ToCharNullableMethod;
            }
            if (csharpType == typeof(string))
            {
                return DataConvertMethod.ToStringMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(Guid) || GetUnderlyingType(csharpType) == typeof(Guid))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToGuidMethod : DataConvertMethod.ToGuidNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(DateTime) || GetUnderlyingType(csharpType) == typeof(DateTime))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDateTimeMethod : DataConvertMethod.ToDateTimeNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(byte) || GetUnderlyingType(dbType) == typeof(sbyte) || GetUnderlyingType(csharpType) == typeof(byte) || GetUnderlyingType(csharpType) == typeof(sbyte))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToByteMethod : DataConvertMethod.ToByteNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(short) || GetUnderlyingType(dbType) == typeof(ushort) || GetUnderlyingType(csharpType) == typeof(short) || GetUnderlyingType(csharpType) == typeof(ushort))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn16Method : DataConvertMethod.ToIn16NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(int) || GetUnderlyingType(dbType) == typeof(uint) || GetUnderlyingType(csharpType) == typeof(int) || GetUnderlyingType(csharpType) == typeof(uint))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn32Method : DataConvertMethod.ToIn32NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(long) || GetUnderlyingType(dbType) == typeof(long) || GetUnderlyingType(csharpType) == typeof(long) || GetUnderlyingType(csharpType) == typeof(ulong))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn64Method : DataConvertMethod.ToIn64NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(float) || GetUnderlyingType(csharpType) == typeof(float))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToFloatMethod : DataConvertMethod.ToFloatNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(double) || GetUnderlyingType(csharpType) == typeof(double))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDoubleMethod : DataConvertMethod.ToDoubleNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(decimal) || GetUnderlyingType(csharpType) == typeof(decimal))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDecimalMethod : DataConvertMethod.ToDecimalNullableMethod;
            }
            return !IsNullableType(csharpType) ? DataConvertMethod.ToObjectMethod.MakeGenericMethod(csharpType) : DataConvertMethod.ToObjectNullableMethod.MakeGenericMethod(Nullable.GetUnderlyingType(GetUnderlyingType(csharpType)));
        }

        private Type GetUnderlyingType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType ?? type;
        }

        private bool IsNullableType(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return false;
            }
            return true;
        }

        public EntityMapper(bool matchNamesWithUnderscores = false)
        {
            MatchNamesWithUnderscores = matchNamesWithUnderscores;
        }
    }

    /// <summary>
    /// 数据库类型到Csharp类型转换器
    /// </summary>
    static class DataConvertMethod
    {
        #region Method Field
        public static MethodInfo ToObjectMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToObject));
        public static MethodInfo ToByteMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToByte));
        public static MethodInfo ToIn16Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt16));
        public static MethodInfo ToIn32Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt32));
        public static MethodInfo ToIn64Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt64));
        public static MethodInfo ToFloatMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToFloat));
        public static MethodInfo ToDoubleMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDouble));
        public static MethodInfo ToDecimalMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDecimal));
        public static MethodInfo ToBooleanMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToBoolean));
        public static MethodInfo ToCharMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToChar));
        public static MethodInfo ToStringMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToString));
        public static MethodInfo ToDateTimeMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDateTime));
        public static MethodInfo ToEnumMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToEnum));
        public static MethodInfo ToGuidMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToGuid));
        #endregion

        #region NullableMethod Field
        public static MethodInfo ToObjectNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertObjectNullable));
        public static MethodInfo ToByteNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToByteNullable));
        public static MethodInfo ToIn16NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt16Nullable));
        public static MethodInfo ToIn32NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt32Nullable));
        public static MethodInfo ToIn64NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt64Nullable));
        public static MethodInfo ToFloatNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToFloatNullable));
        public static MethodInfo ToDoubleNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDoubleNullable));
        public static MethodInfo ToBooleanNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToBooleanNullable));
        public static MethodInfo ToDecimalNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDecimalNullable));
        public static MethodInfo ToCharNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToCharNullable));
        public static MethodInfo ToDateTimeNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDateTimeNullable));
        public static MethodInfo ToEnumNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToEnumNullable));
        public static MethodInfo ToGuidNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToGuidNullable));
        #endregion

        #region Define Convert
        public static T ConvertToObject<T>(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                var data = dr.GetValue(i);
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch
            {
                throw ThrowException<T>(dr, i);
            }
        }

        public static byte ConvertToByte(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                var result = dr.GetByte(i);
                return result;
            }
            catch
            {
                throw ThrowException<byte>(dr, i);
            }
        }

        public static short ConvertToInt16(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(short))
                {
                    return dr.GetInt16(i);
                }
                return Convert.ToInt16(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<short>(dr, i);
            }
        }

        public static int ConvertToInt32(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(int))
                {
                    return dr.GetInt32(i);
                }
                return Convert.ToInt32(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<int>(dr, i);
            }
        }

        public static long ConvertToInt64(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(long))
                {
                    return dr.GetInt64(i);
                }
                return Convert.ToInt64(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<long>(dr, i);
            }
        }

        public static float ConvertToFloat(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(float))
                {
                    return dr.GetFloat(i);
                }
                return Convert.ToSingle(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<float>(dr, i);
            }
        }

        public static double ConvertToDouble(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(double))
                {
                    return dr.GetDouble(i);
                }
                return Convert.ToDouble(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<double>(dr, i);
            }
        }

        public static bool ConvertToBoolean(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                if (dr.GetFieldType(i) == typeof(bool))
                {
                    var result = dr.GetBoolean(i);
                    return result;
                }
                else if (int.TryParse(dr.GetValue(i).ToString(), out int value))
                {
                    return !(value == 0);
                }
                return Convert.ToBoolean(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<bool>(dr, i);
            }
        }

        public static decimal ConvertToDecimal(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(decimal))
                {
                    return dr.GetDecimal(i);
                }
                return Convert.ToDecimal(dr.GetValue(i));
            }
            catch
            {
                throw ThrowException<bool>(dr, i);
            }
        }

        public static char ConvertToChar(this IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                var result = dr.GetChar(i);
                return result;
            }
            catch
            {
                throw ThrowException<char>(dr, i);
            }
        }

        public static string ConvertToString(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(string))
                {
                    return dr.GetString(i);
                }
                var result = dr.GetValue(i);
                return Convert.ToString(result);
            }
            catch
            {
                throw ThrowException<string>(dr, i);
            }
        }     

        public static DateTime ConvertToDateTime(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                else if (dr.GetFieldType(i) == typeof(DateTime))
                {
                    return dr.GetDateTime(i);
                }
                return DateTime.Parse(dr.GetValue(i).ToString());
            }
            catch
            {
                throw ThrowException<DateTime>(dr, i);
            }
        }

        public static T ConvertToEnum<T>(IDataRecord dr, int i) where T : struct
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                var value = dr.GetValue(i);
                if (Enum.TryParse(value.ToString(), out T result)) return result;
                return default;
            }
            catch
            {
                throw ThrowException<T>(dr, i);
            }
        }

        public static Guid ConvertToGuid(IDataRecord dr, int i)
        {
            try
            {
                if (dr.IsDBNull(i))
                {
                    return default;
                }
                var result = dr.GetGuid(i);
                return result;
            }
            catch
            {
                throw ThrowException<Guid>(dr, i);
            }
        }

        private static Exception ThrowException<T>(IDataRecord dr, int i)
        {
            var inner = new FormatException($"Column of {dr.GetName(i)} {dr.GetFieldType(i)} '{dr.GetValue(i)}' was not recognized as a valid {typeof(T).Name}.");
            return new InvalidCastException($"Unable to cast object of type '{dr.GetFieldType(i).Name}' to type '{typeof(int).Name}'.", inner);
        }
        #endregion

        #region Define Nullable Convert
        public static T ConvertObjectNullable<T>(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToObject<T>(dr, i);
        }
        public static byte? ConvertToByteNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToByte(dr, i);
        }
        public static short? ConvertToInt16Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToInt16(dr, i);
        }
        public static int? ConvertToInt32Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToInt32(dr, i);
        }
        public static long? ConvertToInt64Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToInt64(dr, i);
        }
        public static float? ConvertToFloatNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToFloat(dr, i);
        }
        public static double? ConvertToDoubleNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToDouble(dr, i);
        }
        public static bool? ConvertToBooleanNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToBoolean(dr, i);
        }
        public static decimal? ConvertToDecimalNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToDecimal(dr, i);
        }
        public static char? ConvertToCharNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToChar(dr, i);
        }    
        public static DateTime? ConvertToDateTimeNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToDateTime(dr, i);
        }
        public static T? ConvertToEnumNullable<T>(IDataRecord dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToEnum<T>(dr, i);
        }
        public static Guid? ConvertToGuidNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return ConvertToGuid(dr, i);
        }
        #endregion
    }
}
