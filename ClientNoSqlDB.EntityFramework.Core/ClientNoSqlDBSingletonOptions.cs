using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClientNoSqlDB.EntityFramework.Core
{
    public class ClientNoSqlDBSingletonOptions : IClientNoSqlDBSingletonOptions
    {
        public bool IsNullabilityCheckEnabled { get; set; }

        public void Initialize(IDbContextOptions options)
        {
            throw new NotImplementedException();
        }

        public void Validate(IDbContextOptions options)
        {
            throw new NotImplementedException();
        }
    }

}
