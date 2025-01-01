using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace ClientNoSqlDB.EntityFramework.Core.Storage
{
    public class ClientNoSqlDBDatabase : Database
    {
        private readonly IDesignTimeModel _designTimeModel;
        private readonly IUpdateAdapterFactory _updateAdapterFactory;

        public ClientNoSqlDBDatabase(DatabaseDependencies dependencies,
                                     IDesignTimeModel designTimeModel,
                                     IUpdateAdapterFactory updateAdapterFactory)
            : base(dependencies)
        {
            _designTimeModel = designTimeModel;
            _updateAdapterFactory = updateAdapterFactory;
        }
        public override int SaveChanges(IList<IUpdateEntry> entries)
        {
            throw new NotImplementedException();
        }

        public override Task<int> SaveChangesAsync(IList<IUpdateEntry> entries, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
