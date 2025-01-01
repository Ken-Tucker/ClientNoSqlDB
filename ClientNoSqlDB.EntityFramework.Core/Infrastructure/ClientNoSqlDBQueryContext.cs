using Microsoft.EntityFrameworkCore.Query;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBQueryContext : QueryContext
    {
        public ClientNoSqlDBQueryContext(QueryContextDependencies dependencies) : base(dependencies)
        {
        }
    }
}
