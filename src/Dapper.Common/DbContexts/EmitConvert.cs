using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Dapper
{
    /// <summary>
    /// DbColumn Information
    /// </summary>
    public class DbDataInfo
    {
        public string TypeName { get; set; }
        public Type DataType { get; set; }
        public string DataName { get; set; }
        public int Ordinal { get; set; }
        public DbDataInfo(int ordinal, string typeName, Type dataType, string dataName)
        {
            Ordinal = ordinal;
            TypeName = typeName;
            DataType = dataType;
            DataName = dataName;
        }
    }
    /// <summary>
    /// IL Convert
    /// </summary>
    public class EmitConvert
    {
        private readonly static ConcurrentDictionary<SerializerKey, object> _serializers
            = new ConcurrentDictionary<SerializerKey, object>();
        private readonly static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers
            = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();

        private struct SerializerKey : IEquatable<SerializerKey>
        {
            private string[] Names { get; set; }
            private Type Type { get; set; }
            public override bool Equals(object obj)
            {
                return obj is SerializerKey && Equals((SerializerKey)obj);
            }
            public bool Equals(SerializerKey other)
            {
                if (Type != other.Type)
                {
                    return false;
                }
                else if (Names == other.Names)
                {
                    return true;
                }
                else if (Names.Length != other.Names.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < Names.Length; i++)
                    {
                        if (Names[i] != other.Names[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            public override int GetHashCode()
            {
                return Type.GetHashCode();
            }
            public SerializerKey(Type type, string[] names)
            {
                Type = type;
                Names = names;
            }
        }
        /// <summary>
        /// IDataRecord Converted to T
        /// </summary>
        public static Func<IDataRecord, T> GetSerializer<T>(IEntityMapperProvider mapper, IDataRecord record)
        {
            string[] names = new string[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++)
            {
                names[i] = record.GetName(i);
            }
            var key = new SerializerKey(typeof(T), names.Length == 1 ? null : names);
            var handler = _serializers.GetOrAdd(key, k =>
             {
                 return CreateTypeSerializerHandler<T>(mapper, record);
             });
            return handler as Func<IDataRecord, T>;
        }
        /// <summary>
        /// IDataRecord Converted to dynamic
        /// </summary>
        public static Func<IDataRecord, dynamic> GetSerializer()
        {
            return (reader) =>
            {
                dynamic obj = new System.Dynamic.ExpandoObject();
                var row = (IDictionary<string, object>)obj;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    row.Add(name, reader.GetValue(i));
                }
                return row;
            };
        }
        /// <summary>
        /// IDataRecord Converted to T
        /// </summary>
        public static Func<IDataRecord, T> GetSerializer<T>(IDataRecord record)
        {
            return GetSerializer<T>(GlobalSettings.EntityMapperProvider, record);
        }
        /// <summary>
        /// Object To Dictionary&lt;tstring, object&gt;
        /// </summary>
        public static Func<object, Dictionary<string, object>> GetDeserializer(Type type)
        {
            if (type == typeof(Dictionary<string, object>))
            {
                return (object param) => param as Dictionary<string, object>;
            }
            var handler = _deserializers.GetOrAdd(type, t =>
            {
                return CreateTypeDeserializerHandler(type) as Func<object, Dictionary<string, object>>;
            });
            return handler;
        }
        private static Func<object, Dictionary<string, object>> CreateTypeDeserializerHandler(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var methodName = $"{type.Name}Deserializer{Guid.NewGuid():N}";
            var dynamicMethod = new DynamicMethod(methodName, typeof(Dictionary<string, object>), new Type[] { typeof(object) }, type, true);
            var generator = dynamicMethod.GetILGenerator();
            LocalBuilder entityLocal1 = generator.DeclareLocal(typeof(Dictionary<string, object>));
            LocalBuilder entityLocal2 = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, entityLocal1);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, type);
            generator.Emit(OpCodes.Stloc, entityLocal2);
            foreach (var item in properties)
            {
                generator.Emit(OpCodes.Ldloc, entityLocal1);
                generator.Emit(OpCodes.Ldstr, item.Name);
                generator.Emit(OpCodes.Ldloc, entityLocal2);
                generator.Emit(OpCodes.Callvirt, item.GetGetMethod());
                if (item.PropertyType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, item.PropertyType);
                }
                var addMethod = typeof(Dictionary<string, object>).GetMethod(nameof(Dictionary<string, object>.Add), new Type[] { typeof(string), typeof(object) });
                generator.Emit(OpCodes.Callvirt, addMethod);
            }
            generator.Emit(OpCodes.Ldloc, entityLocal1);
            generator.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<object, Dictionary<string, object>>)) as Func<object, Dictionary<string, object>>;
        }

        private static Func<IDataRecord, T> CreateTypeSerializerHandler<T>(IEntityMapperProvider mapper, IDataRecord record)
        {
            var type = typeof(T);
            var methodName = $"{type.Name}Serializer{Guid.NewGuid():N}";
            var dynamicMethod = new DynamicMethod(methodName, type, new Type[] { typeof(IDataRecord) }, type, true);
            var generator = dynamicMethod.GetILGenerator();
            LocalBuilder local = generator.DeclareLocal(type);
            var dataInfos = new DbDataInfo[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++)
            {
                var dataname = record.GetName(i);
                var datatype = record.GetFieldType(i);
                var typename = record.GetDataTypeName(i);
                dataInfos[i] = new DbDataInfo(i, typename, datatype, dataname);
            }
            /*
             * T t;
             * t = ConvertTo***(reader,i);
             * return t;
             */
            if (dataInfos.Length == 1 && (type.IsValueType || type == typeof(string) || type == typeof(object)))
            {
                var dataInfo = dataInfos.First();
                var convertMethod = mapper.FindConvertMethod(type, dataInfo.DataType);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, 0);
                if (convertMethod.IsVirtual)
                    generator.Emit(OpCodes.Callvirt, convertMethod);
                else
                    generator.Emit(OpCodes.Call, convertMethod);
                if (type == typeof(object) && convertMethod.ReturnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, convertMethod.ReturnType);
                }
                generator.Emit(OpCodes.Stloc, local);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
            }
            var constructor = mapper.FindConstructor(type);
            /*
            * T t;
            * int a;
            * int b;
            * a = ConvertTo**(reader,i);
            * b = ConvertTo***(reader,i);
            * t = new T(a,b);
            * return t;
            */
            if (constructor.GetParameters().Length > 0)
            {
                var parameters = constructor.GetParameters();
                var locals = new LocalBuilder[parameters.Length];
                for (int i = 0; i < locals.Length; i++)
                {
                    locals[i] = generator.DeclareLocal(parameters[i].ParameterType);
                }
                for (int i = 0; i < locals.Length; i++)
                {
                    var item = mapper.FindConstructorParameter(dataInfos, parameters[i]);
                    if (item == null)
                    {
                        continue;
                    }
                    var convertMethod = mapper.FindConvertMethod(parameters[i].ParameterType, item.DataType);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, item.Ordinal);
                    if (convertMethod.IsVirtual)
                        generator.Emit(OpCodes.Callvirt, convertMethod);
                    else
                        generator.Emit(OpCodes.Call, convertMethod);
                    generator.Emit(OpCodes.Stloc, locals[i]);
                }
                for (int i = 0; i < locals.Length; i++)
                {
                    generator.Emit(OpCodes.Ldloc, locals[i]);
                }
                generator.Emit(OpCodes.Newobj, constructor);
                generator.Emit(OpCodes.Stloc, local);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
            }
            /*
             * T t;
             * t = new T();
             * t.A = ConvertTo**(reader,i);
             * t.B = = ConvertTo***(reader,i);
             * return t;
             */
            else
            {
                var properties = type.GetProperties();
                generator.Emit(OpCodes.Newobj, constructor);
                generator.Emit(OpCodes.Stloc, local);
                foreach (var item in dataInfos)
                {
                    var property = mapper.FindMember(properties, item) as PropertyInfo;
                    if (property == null)
                    {
                        continue;
                    }
                    var convertMethod = mapper.FindConvertMethod(property.PropertyType, item.DataType);
                    if (convertMethod == null)
                    {
                        continue;
                    }
                    int i = record.GetOrdinal(item.DataName);
                    generator.Emit(OpCodes.Ldloc, local);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    if (convertMethod.IsVirtual)
                        generator.Emit(OpCodes.Callvirt, convertMethod);
                    else
                        generator.Emit(OpCodes.Call, convertMethod);
                    generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
                }
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
            }
        }
    }
}
