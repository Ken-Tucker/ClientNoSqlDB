﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClientNoSql.Tests
{
  /// <summary>
  /// Pure POCO data entity
  /// </summary>
  public class MyData 
  {
    public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
        public string LastName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public short ShortField { get; set; }
    public short? ShortNField { get; set; }

    public ushort UShortField { get; set; }
    public ushort? UShortNField { get; set; }

    public int IntField { get; set; }
    public int? IntNField { get; set; }

    public uint UIntField { get; set; }
    public uint? UIntNField { get; set; }

    public long LongField { get; set; }
    public long? LongNField { get; set; }

    public ulong ULongField { get; set; }
    public ulong? ULongNField { get; set; }

    public double DoubleField { get; set; }
    public double? DoubleNField { get; set; }

    public decimal DecimalField { get; set; }
    public decimal? DecimalNField { get; set; }

    public float FloatField { get; set; }
    public float? FloatNField { get; set; }

    public bool BoolField { get; set; }
    public bool? BoolNField { get; set; }

    public DateTime DateTimeField { get; set; }
    public DateTime? DateTimeNField { get; set; }

    public DateTimeOffset DateTimeOffsetField { get; set; }
    public DateTimeOffset? DateTimeOffsetNField { get; set; }

    public TimeSpan TimeSpanField { get; set; }
    public TimeSpan? TimeSpanNField { get; set; }

    public Guid GuidField { get; set; }
    public Guid? GuidNField { get; set; }

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
   public List<int> ListField { get; set; }

    public Dictionary<string, int> DictField { get; set; }
    public ObservableCollection<int> CollectionField { get; set; }

    public TestEnum EnumField { get; set; }
    public TestEnum? EnumNField { get; set; }

    public byte[] BlobField;

        public StringBuilder StringBuilderField;
        public UriBuilder UriBuilderField;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  }

  /// <summary>
  /// Test enumeration for serialization roundtrip
  /// </summary>
  public enum TestEnum
  {
    None,
    EnumValue1,
    EnumValue2
  }

  /// <summary>
  /// Pure POCO with references
  /// </summary>
  public class MyDataGroup
  {
    public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Name { get; set; }
    public List<MyData> Items { get; set; }
        public MyDataGroup Parent { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

  /// <summary>
  /// Interface for interface based data entity 
  /// </summary>
  public interface IData
  {
    int Id { get; set; }
    string Name { get; set; }
  }

  /// <summary>
  /// Implementation for interface based data entity
  /// </summary>
  public class InterfaceBasedData: IData
  {
    public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

  /// <summary>
  /// Prototype for prototype based data entity
  /// </summary>
  public abstract class AData
  {
    public abstract int Id { get; set; }
    public abstract string Name { get; set; }
  }

  /// <summary>
  /// Implementation for prototype based data entity
  /// </summary>
  public class PrototypeBasedData: AData
  {
    public override int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public override string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

  public class MyDataKeys
  {
    public bool KeyBool;
    public bool? KeyBoolN;

    public int KeyInt;
    public int? KeyIntN;

    public long KeyLong;
    public long? KeyLongN;

    public Guid KeyGuid;
    public Guid? KeyGuidN;

    public TimeSpan KeyTimeSpan;
    public TimeSpan? KeyTimeSpanN;

    public DateTime KeyDateTime;
    public DateTime? KeyDateTimeN;

    public DateTimeOffset KeyDateTimeOffset;
    public DateTimeOffset? KeyDateTimeOffsetN;
  }
}
