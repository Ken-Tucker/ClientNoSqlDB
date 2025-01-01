using Microsoft.EntityFrameworkCore.Query;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure.Query
{
    public class ClientNoSqlDBQueryableMethodTranslatingExpressionVisitorFactory : IQueryableMethodTranslatingExpressionVisitorFactory
    {
        public QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        {
            throw new NotImplementedException();
        }
    }
}
