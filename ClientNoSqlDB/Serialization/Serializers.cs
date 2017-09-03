﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ClientNoSqlDB.Serialization
{
  using Indexing;

  /// <summary>
  /// Serialization extender
  /// </summary>
  public static class Extender
  {
    /// <summary>
    /// Registers static serialization extension before any DbInstance initialization.
    /// Serialization extensions is a static class with following methods pattern:
    /// public static K ReadXXX(DataReader reader)
    /// public static void WriteXXX(DataWriter writer, K value)
    /// where K is custom type to serialize
    /// XXX is type name of K without namespace
    /// </summary>
    /// <typeparam name="K">Type to serialize</typeparam>
    /// <typeparam name="S">Serialization extension</typeparam>
    /// <param name="streamId">Stable (for the lifetime of your app) id of the custom type. 
    /// Must be greater than 1000 (other are already taken or reserved for future use)</param>
    public static void RegisterType<K, S>(short streamId)
    {
      Serializers.RegisterType<K, S>(streamId);
    }
  }

  /// <summary>
  /// Provides serialization logic and code generation
  /// </summary>
  static class Serializers
  {
    public static Type GetBinaryType(Type type)
    {
      var nt = Nullable.GetUnderlyingType(type);
      if (nt != null)
        type = nt;

      if (type.IsEnum())
        return Enum.GetUnderlyingType(type);

      return type;
    }

    static MethodInfo GetWriteMethodFrom<K, S>()
    {
      var result = typeof(S).GetPublicStaticMethod("Write" + typeof(K).Name);
      if (result == null)
        throw new ArgumentException("Write method not found");

      if (result.ReturnType != typeof(void))
        throw new ArgumentException("Write method return type mismatch");

      var parameters = result.GetParameters();
      if (parameters.Length != 2 || parameters[0].ParameterType != typeof(DataWriter) || parameters[1].ParameterType != typeof(K))
        throw new ArgumentException("Write method parameter type mismatch");

      return result;
    }

    static MethodInfo GetReadMethodFrom<K, S>()
    {
      var result = typeof(S).GetPublicStaticMethod("Read" + typeof(K).Name);
      if (result == null)
        throw new ArgumentException("Read method not found");

      if (result.ReturnType != typeof(K))
        throw new ArgumentException("Read method return type mismatch");

      var parameters = result.GetParameters();
      if (parameters.Length != 1 || parameters[0].ParameterType != typeof(DataReader))
        throw new ArgumentException("Read method parameter type mismatch");

      return result;
    }

    internal static void RegisterType<K, S>(short streamId)
    {
      if (Enum.IsDefined(typeof(KnownDbType), streamId))
        throw new ArgumentException("streamId");

      var reader = GetReadMethodFrom<K, S>();
      var writer = GetWriteMethodFrom<K, S>();

      DbTypes.Register<K>(streamId);


      var readerMethod = reader;
      var writerMethod = writer;


      lock (_readerMethods)
        _readerMethods.Add(typeof(K), readerMethod);

      lock (_writerMethods)
        _writerMethods.Add(typeof(K), writerMethod);
    }


    static readonly Dictionary<Type, MethodInfo> _readerMethods;
    static readonly Dictionary<Type, MethodInfo> _writerMethods;

    static Serializers()
    {
      var methods = typeof(Serializers).GetStaticMethods().ToArray();

      _readerMethods = (from m in methods
                        where m.Name.StartsWith("Read") && m.ReturnType != typeof(void)
                        let parameters = m.GetParameters()
                        where parameters.Length == 1 && parameters[0].ParameterType == typeof(DataReader)
                        select m).ToDictionary(i => i.ReturnType);

      _writerMethods = (from m in methods
                        where m.Name.StartsWith("Write") && m.ReturnType == typeof(void)
                        let parameters = m.GetParameters()
                        where parameters.Length == 2 && parameters[0].ParameterType == typeof(DataWriter)
                        select new { Method = m, Type = parameters[1].ParameterType }).ToDictionary(i => i.Type, i => i.Method);
    }

    static readonly MethodInfo _writeBool = typeof(BinaryWriter).GetMethod("Write", new[] { typeof(bool) });
    static readonly MethodInfo _readBool = typeof(BinaryReader).GetMethod("ReadBoolean");
    static readonly MethodInfo _hasValue = typeof(Serializers).GetMethod("HasValue");

    internal static Expression WriteValue(Expression writer, Expression value)
    {
      if (!value.Type.IsValueType())
        return WriteValueReference(writer, value);

      var nn = Nullable.GetUnderlyingType(value.Type);
      if (nn == null) // not nullable
        return WriteValueNormal(writer, value);

      return WriteValueNullable(writer, value, nn);
    }

    static Expression WriteValueNormal(Expression writer, Expression value)
    {
      var type = GetBinaryType(value.Type);
      if (type != value.Type)
        value = Expression.Convert(value, type);

      var writeNotNull = Expression.Call(writer, _writeBool, Expression.Constant(false));
      var writeValue = Expression.Call(null, GetWriteMethod(type), writer, value);  // Serializers.WriteXXX(writer, obj.Property|Field);

      return Expression.Block(writeNotNull, writeValue);
    }

    static bool HasValue<T>(T? value) where T : struct
    {
      return value.HasValue;
    }

    static Expression WriteValueNullable(Expression writer, Expression value, Type nnType)
    {
      var @cond = Expression.Property(value, "HasValue");
      var @then = WriteValueNormal(writer, Expression.Property(value, "Value"));
      var @else = Expression.Call(writer, _writeBool, Expression.Constant(true));

      return Expression.IfThenElse(@cond, @then, @else);
    }

    static Expression WriteValueReference(Expression writer, Expression value)
    {
      var @cond = Expression.Equal(value, Expression.Constant(null));
      var @then = Expression.Call(writer, _writeBool, Expression.Constant(true));
      var @else = WriteValueNormal(writer, value);

      return Expression.IfThenElse(@cond, @then, @else);
    }

    internal static Expression ReadValue(Expression reader, Type type)
    {
      var nn = Nullable.GetUnderlyingType(type);
      return ReadValue(reader, type, nn ?? type);
    }

    static Expression ReadValue(Expression reader, Type resultType, Type dataType)
    {
      var @cond = Expression.Call(reader, _readBool);
      var @then = Expression.Default(resultType);
      var @else = ReadValueDirect(reader, dataType);

      if (dataType != resultType)
        @else = Expression.Convert(@else, resultType);

      return Expression.Condition(@cond, @then, @else);
    }

    static Expression ReadValueDirect(Expression reader, Type type)
    {
      var dataType = GetBinaryType(type);
      var readMethod = GetReadMethod(dataType);

      var callRead = Expression.Call(null, readMethod, reader);

      if (dataType != type)
        return Expression.Convert(callRead, type);

      return callRead;
    }

    public static MethodInfo GetWriteMethod(Type type)
    {
      lock (_writerMethods)
      {
        MethodInfo result;

        if (_writerMethods.TryGetValue(type, out result))
          return result;

        if (type.IsGenericType())
          return _writerMethods[type] = MakeGenericWrite(type);

        if (type.IsArray)
          return _writerMethods[type] = MakeArrayWrite(type);
      }
      throw new NotSupportedException();
    }

    public static MethodInfo GetReadMethod(Type type)
    {
      lock (_readerMethods)
      {
        MethodInfo result;
        if (_readerMethods.TryGetValue(type, out result))
          return result;

        if (type.IsGenericType())
          return _readerMethods[type] = MakeGenericRead(type);

        if (type.IsArray)
          return _readerMethods[type] = MakeArrayRead(type);
      }
      throw new NotSupportedException();
    }

    static MethodInfo MakeArrayWrite(Type type)
    {
      return MakeArrayWriteCore(typeof(ListSerializers<>).MakeGenericType(type.GetElementType()));
    }

    static MethodInfo MakeArrayWriteCore(Type type)
    {
      return type.GetStaticMethod("WriteArray");
    }

    static MethodInfo MakeArrayRead(Type type)
    {
      return MakeArrayReadCore(typeof(ListSerializers<>).MakeGenericType(type.GetElementType()));
    }

    static MethodInfo MakeArrayReadCore(Type type)
    {
      return type.GetStaticMethod("ReadArray");
    }

    static MethodInfo MakeGenericWrite(Type type)
    {
      var baseK = type.GetGenericTypeDefinition();

      if (baseK == typeof(Indexer<,>) || baseK == typeof(Indexer<,,>))
        return type.GetStaticMethod("Serialize");

      if (baseK == typeof(HashSet<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("WriteHashSet");

      if (baseK == typeof(List<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("WriteList");



      if (baseK == typeof(ObservableCollection<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("WriteCollection");


      if (baseK == typeof(Dictionary<,>))
        return typeof(DictSerializers<,>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("WriteDictionary");

      throw new NotSupportedException();
    }

    static MethodInfo MakeGenericRead(Type type)
    {
      var baseK = type.GetGenericTypeDefinition();

      if (baseK == typeof(Indexer<,>) || baseK == typeof(Indexer<,,>))
        return type.GetStaticMethod("Deserialize");

      if (baseK == typeof(HashSet<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("ReadHashSet");

      if (baseK == typeof(List<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("ReadList");



      if (baseK == typeof(ObservableCollection<>))
        return typeof(ListSerializers<>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("ReadCollection");


      if (baseK == typeof(Dictionary<,>))
        return typeof(DictSerializers<,>).MakeGenericType(type.GetGenericArguments()).GetStaticMethod("ReadDictionary");

      throw new NotSupportedException();
    }

    #region Guid serialization

    public static Guid ReadGuid(DataReader reader)
    {
      return reader.ReadGuid();
    }

    public static void WriteGuid(DataWriter writer, Guid value)
    {
      writer.Write(value);
    }

    #endregion

    #region Uri serialization

    public static Uri ReadUri(DataReader reader)
    {
      return reader.ReadUri();
    }

    public static void WriteUri(DataWriter writer, Uri value)
    {
      writer.Write(value);
    }

    #endregion

    #region UriBuilder serialization  

    public static UriBuilder ReadUriBuilder(DataReader reader)
    {
      return reader.ReadUriBuilder();
    }

    public static void WriteUriBuilder(DataWriter writer, UriBuilder value)
    {
      writer.Write(value);
    }

    #endregion

    #region short serialization

    public static short ReadShort(DataReader reader)
    {
      return reader.ReadInt16();
    }

    public static void WriteShort(DataWriter writer, short value)
    {
      writer.Write(value);
    }

    #endregion

    #region ushort serialization

    public static ushort ReadUShort(DataReader reader)
    {
      return reader.ReadUInt16();
    }

    public static void WriteUShort(DataWriter writer, ushort value)
    {
      writer.Write(value);
    }

    #endregion

    #region int serialization

    public static int ReadInt(DataReader reader)
    {
      return reader.ReadInt32();
    }

    public static void WriteInt(DataWriter writer, int value)
    {
      writer.Write(value);
    }

    #endregion

    #region uint serialization

    public static uint ReadUInt(DataReader reader)
    {
      return reader.ReadUInt32();
    }

    public static void WriteUInt(DataWriter writer, uint value)
    {
      writer.Write(value);
    }

    #endregion

    #region long serialization

    public static long ReadLong(DataReader reader)
    {
      return reader.ReadInt64();
    }

    public static void WriteLong(DataWriter writer, long value)
    {
      writer.Write(value);
    }

    #endregion

    #region ulong serialization

    public static ulong ReadULong(DataReader reader)
    {
      return reader.ReadUInt64();
    }

    public static void WriteULong(DataWriter writer, ulong value)
    {
      writer.Write(value);
    }

    #endregion

    #region float serialization

    public static float ReadFloat(DataReader reader)
    {
      return reader.ReadSingle();
    }

    public static void WriteFloat(DataWriter writer, float value)
    {
      writer.Write(value);
    }

    #endregion

    #region double serialization

    public static double ReadDouble(DataReader reader)
    {
      return reader.ReadDouble();
    }

    public static void WriteDouble(DataWriter writer, double value)
    {
      writer.Write(value);
    }

    #endregion

    #region string serialization

    public static string ReadString(DataReader reader)
    {
      return reader.ReadString();
    }

    public static void WriteString(DataWriter writer, string value)
    {
      writer.Write(value);
    }

    #endregion

    #region StringBuilder serialization  

    public static StringBuilder ReadStringBuilder(DataReader reader)
    {
      return reader.ReadStringBuilder();
    }

    public static void WriteStringBuilder(DataWriter writer, StringBuilder value)
    {
      writer.Write(value);
    }

    #endregion

    #region DateTime serialization

    public static DateTime ReadDateTime(DataReader reader)
    {
      return reader.ReadDateTime();
    }

    public static void WriteDateTime(DataWriter writer, DateTime value)
    {
      writer.Write(value);
    }

    #endregion

    #region DateTimeOffset serialization

    public static DateTimeOffset ReadDateTimeOffset(DataReader reader)
    {
      return reader.ReadDateTimeOffset();
    }

    public static void WriteDateTimeOffset(DataWriter writer, DateTimeOffset value)
    {
      writer.Write(value);
    }

    #endregion

    #region TimeSpan serialization

    public static TimeSpan ReadTimeSpan(DataReader reader)
    {
      return reader.ReadTimeSpan();
    }

    public static void WriteTimeSpan(DataWriter writer, TimeSpan value)
    {
      writer.Write(value);
    }

    #endregion

    #region bool serialization

    public static bool ReadBoolean(DataReader reader)
    {
      return reader.ReadBoolean();
    }

    public static void WriteBoolean(DataWriter writer, bool value)
    {
      writer.Write(value);
    }

    #endregion

    #region byte serialization

    public static byte ReadByte(DataReader reader)
    {
      return reader.ReadByte();
    }

    public static void WriteByte(DataWriter writer, byte value)
    {
      writer.Write(value);
    }

    #endregion

    #region sbyte serialization

    public static sbyte ReadSByte(DataReader reader)
    {
      return reader.ReadSByte();
    }

    public static void WriteSByte(DataWriter writer, sbyte value)
    {
      writer.Write(value);
    }

    #endregion

    #region byte array serialization

    public static byte[] ReadArray(DataReader reader)
    {
      return reader.ReadArray();
    }

    public static void WriteArray(DataWriter writer, byte[] value)
    {
      writer.WriteArray(value);
    }

    #endregion

    #region decimal serialization

    public static decimal ReadDecimal(DataReader reader)
    {
      return reader.ReadDecimal();
    }

    public static void WriteDecimal(DataWriter writer, decimal value)
    {

      writer.Write(value);

    }

    #endregion

  }

  static class Serializer<K>
  {
    public static readonly Action<DataWriter, K> Writer = GetWriter();
    public static readonly Func<DataReader, K> Reader = GetReader();

    static Action<DataWriter, K> GetWriter()
    {

      var writer = Expression.Parameter(typeof(DataWriter));
      var value = Expression.Parameter(typeof(K));

      return Expression.Lambda<Action<DataWriter, K>>(Serializers.WriteValue(writer, value), writer, value).Compile();
    }

    static Func<DataReader, K> GetReader()
    {

      var reader = Expression.Parameter(typeof(DataReader));

      return Expression.Lambda<Func<DataReader, K>>(Serializers.ReadValue(reader, typeof(K)), reader).Compile();

    }
  }

  /// <summary>
  /// Extended binary reader 
  /// </summary>
  public sealed class DataReader : BinaryReader
  {
    /// <summary>
    /// Creates DataReader with specified owned stream
    /// </summary>
    /// <param name="stream">Stream to read from</param>
    public DataReader(Stream stream) : base(stream) { }

    public UriBuilder ReadUriBuilder()
    {
      var uri = ReadUri();
      if (uri == null)
        return null;

      return new UriBuilder(uri);
    }

    public StringBuilder ReadStringBuilder()
    {
      var result = ReadString();
      if (result == null)
        return null;

      return new StringBuilder(result);
    }

    /// <summary>
    /// Reads TimeSpan value from stream
    /// </summary>
    /// <returns>TimeSpan value</returns>
    public TimeSpan ReadTimeSpan()
    {
      return new TimeSpan(ReadInt64());
    }

    static DateTime RawToDateTime(long value)
    {
      return new DateTime(value & 0x3fffffffffffffffL, (DateTimeKind)((value >> 0x3e) & 0x3));
    }

    public Uri ReadUri()
    {
      var uri = ReadString();
      if (uri == null)
        return null;

      var absolute = ReadBoolean();
      if (absolute)
        return new Uri(uri, UriKind.Absolute);

      return new Uri(uri, UriKind.Relative);
    }

    /// <summary>
    /// Reads DateTime value from stream
    /// </summary>
    /// <returns>DateTime value</returns>
    public DateTime ReadDateTime()
    {
      return RawToDateTime(ReadInt64());
    }

    /// <summary>
    /// Reads DateTimeOffset value from stream
    /// </summary>
    /// <returns>DateTimeOffset value</returns>
    public DateTimeOffset ReadDateTimeOffset()
    {
      var date = ReadDateTime();
      var offset = ReadInt16();
      return new DateTimeOffset(date, new TimeSpan(0, offset, 0));
    }

    /// <summary>
    /// Reads Guid value from stream
    /// </summary>
    /// <returns>Guid value</returns>
    public Guid ReadGuid()
    {
      return new Guid(ReadBytes(16));
    }

    /// <summary>
    /// Reads byte array from stream
    /// </summary>
    /// <returns>Byte array</returns>
    public byte[] ReadArray()
    {
      return ReadBytes(ReadInt32());
    }


    
    /// <summary>
    /// Reads Decimal value from stream
    /// </summary>
    /// <returns>Decimal value</returns>
    public decimal ReadDecimal()
    {
      return new decimal(new[] { ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32() });
    }

  }

  /// <summary>
  /// Extended binary writer
  /// </summary>
  public sealed class DataWriter : BinaryWriter
  {
    /// <summary>
    /// Creates DataWriter with specified owned stream
    /// </summary>
    /// <param name="stream">Stream to write to</param>
    public DataWriter(Stream stream) : base(stream) { }

    public void Write(UriBuilder value)
    {
      if (value == null)
        Write((string)null);
      else
      {
        var uri = value.Uri;
        var absolute = uri.IsAbsoluteUri;
        Write(absolute);
        Write(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped));
      }
    }

    public void Write(StringBuilder value)
    {
      if (value == null)
        Write((string)null);
      else
        Write(value.ToString());
    }

    /// <summary>
    /// Writes TimeSpan value to stream
    /// </summary>
    /// <param name="value">TimeSpan value to write</param>
    public void Write(TimeSpan value)
    {
      Write(value.Ticks);
    }

    static long DateTimeToRaw(DateTime value)
    {
      return value.Ticks | (((long)value.Kind) << 0x3e);
    }

    /// <summary>
    /// Writes DateTime value to stream
    /// </summary>
    /// <param name="value">DateTime value to write</param>
    public void Write(DateTime value)
    {
      Write(DateTimeToRaw(value));
    }

    /// <summary>
    /// Writes DateTimeOffset value to stream
    /// </summary>
    /// <param name="value">DateTimeOffset value to write</param>
    public void Write(DateTimeOffset value)
    {
      Write(value.DateTime);
      Write((short)(value.Offset.Ticks / TimeSpan.TicksPerMinute));
    }

    public void Write(Uri value)
    {
      if (value == null)
        Write((string)null);
      else
      {
        var absolute = value.IsAbsoluteUri;
        Write(absolute);
        Write(value.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped));
      }
    }

    /// <summary>
    /// Writes Guid value to stream
    /// </summary>
    /// <param name="value">Guid value to write</param>
    public void Write(Guid value)
    {
      Write(value.ToByteArray(), 0, 16);
    }


    /// <summary>
    /// Writes Decimal value to stream
    /// </summary>
    /// <param name="value">Decimal value to write</param>
    public void WriteDecimal(decimal value)
    {
      var bits = decimal.GetBits(value);
      Write(bits[0]);
      Write(bits[1]);
      Write(bits[2]);
      Write(bits[3]);
    }


    /// <summary>
    /// Writes byte array to stream
    /// </summary>
    /// <param name="value">Byte array to write</param>
    public void WriteArray(byte[] value)
    {
      Write(value.Length);
      Write(value);
    }
  }
}
