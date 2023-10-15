using System.Collections.Generic;

namespace ClientNoSqlDB.Serialization
{
    internal class DbDictType : DbType
  {
    public DbDictType(DbType key, DbType value)
      : base((short)KnownDbType.Dict, typeof(Dictionary<,>).MakeGenericType(key.Type, value.Type))
    {
      Key = key;
      Value = value;
    }

    public override void Write(DataWriter writer)
    {
      writer.Write(Id);
      Key.Write(writer);
      Value.Write(writer);
    }

    public override bool AreEqual(DbType type)
    {
      return type.Id == Id && ((DbDictType)type).Key.AreEqual(Key) && ((DbDictType)type).Value.AreEqual(Value);
    }

    public readonly DbType Key, Value;
  }
}
