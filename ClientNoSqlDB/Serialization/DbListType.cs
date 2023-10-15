namespace ClientNoSqlDB.Serialization
{
    internal class DbListType : DbType
    {
        public DbListType(DbType element)
          : base((short)KnownDbType.List, element.Type.MakeArrayType())
        {
            Element = element;
        }

        public override void Write(DataWriter writer)
        {
            writer.Write(Id);
            Element.Write(writer);
        }

        public override bool AreEqual(DbType type)
        {
            return type.Id == Id && ((DbListType)type).Element.AreEqual(Element);
        }

        public readonly DbType Element;
    }
}
