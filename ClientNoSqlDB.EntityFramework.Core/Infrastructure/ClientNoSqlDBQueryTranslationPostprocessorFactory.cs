using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBQueryTranslationPostprocessorFactory : QueryTranslationPostprocessorFactory
    {
        public ClientNoSqlDBQueryTranslationPostprocessorFactory(QueryTranslationPostprocessorDependencies dependencies) : base(dependencies)
        {
        }
    }
}
