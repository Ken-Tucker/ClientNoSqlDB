using Microsoft.EntityFrameworkCore.Query;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBQueryTranslationPostprocessor : QueryTranslationPostprocessor
    {
        public ClientNoSqlDBQueryTranslationPostprocessor(QueryTranslationPostprocessorDependencies dependencies, QueryCompilationContext queryCompilationContext) : base(dependencies, queryCompilationContext)
        {
        }
    }
}
