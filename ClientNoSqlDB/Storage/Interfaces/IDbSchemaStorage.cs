namespace ClientNoSqlDB
{
  interface IDbSchemaStorage
  {
    string Path { get; }

    void Open();

    void Purge();

    IDbTableStorage GetTable(string name);
  }
}
