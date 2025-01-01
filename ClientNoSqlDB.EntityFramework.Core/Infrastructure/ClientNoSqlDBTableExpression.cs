using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBTableExpression : Expression, IPrintableExpression
    {
        public ClientNoSqlDBTableExpression(IEntityType entityType)
        {
            EntityType = entityType;
        }

        public override Type Type => typeof(IEnumerable<ValueBuffer>);

        public virtual IEntityType EntityType { get; }

        public override sealed ExpressionType NodeType => ExpressionType.Extension;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }
        public void Print(ExpressionPrinter expressionPrinter)
        {
            throw new NotImplementedException();
        }
    }
}
