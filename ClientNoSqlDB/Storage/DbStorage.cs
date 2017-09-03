using System;
using System.IO;


namespace ClientNoSqlDB
{
  class DbStorage : IDbStorage
  {

    public IDbSchemaStorage OpenSchema(string path, object home)
    {

      path = Path.Combine("ClientNoSqlDB", path);
      var root = home as string;


      if (root == null)
        root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

      return new FileSystem.DbSchemaStorage(Path.Combine(root, path));

    }

    public bool IncreaseQuotaTo(long quota)
    {

      return true;

    }

    public bool HasEnoughQuota(long quota)
    {

      return true;
    }
  }
}
