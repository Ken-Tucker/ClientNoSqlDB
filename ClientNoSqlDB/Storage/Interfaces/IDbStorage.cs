namespace ClientNoSqlDB
{
  interface IDbStorage
  {
    IDbSchemaStorage OpenSchema(string path, object home = null);

    bool IncreaseQuotaTo(long quota);

    bool HasEnoughQuota(long quota);
  }
}
