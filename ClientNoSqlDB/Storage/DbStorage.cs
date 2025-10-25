using System;
using System.IO;


namespace ClientNoSqlDB
{
    class DbStorage : IDbStorage
    {

        public IDbSchemaStorage OpenSchema(string path, object home)
        {

#if NET8_0_OR_GREATER
            path = Path.Join("ClientNoSqlDB", path);
#else
            path = Path.Combine("ClientNoSqlDB", path);
#endif
            var root = home as string;


            if (root == null)
                root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);


#if NET8_0_OR_GREATER
            return new FileSystem.DbSchemaStorage(Path.Join(root, path));
#else
            return new FileSystem.DbSchemaStorage(Path.Combine(root, path));
#endif     
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
