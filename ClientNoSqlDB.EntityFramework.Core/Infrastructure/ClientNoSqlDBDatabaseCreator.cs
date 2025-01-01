using Microsoft.EntityFrameworkCore.Storage;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBDatabaseCreator : IDatabaseCreator
    {
        public bool CanConnect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool EnsureCreated()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool EnsureDeleted()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
