using Microsoft.EntityFrameworkCore.Storage;

namespace ClientNoSqlDB.EntityFramework.Core.Extensions
{
    public class ClientNoSqlDBTransactionManager : IDbContextTransactionManager
    {
        public IDbContextTransaction? CurrentTransaction => throw new NotImplementedException();

        public IDbContextTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void ResetState()
        {
            throw new NotImplementedException();
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
