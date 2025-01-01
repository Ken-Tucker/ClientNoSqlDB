using ClientNoSqlDB.EntityFramework.Core.Infrastructure;
using ClientNoSqlDB.EntityFramework.Core.Infrastructure.Query;
using ClientNoSqlDB.EntityFramework.Core.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ClientNoSqlDB.EntityFramework.Core.Extensions
{
    public static class ClientNoSqlDBServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkClientNoSqlDBDatabase(this IServiceCollection serviceCollection)
        {
            var builder = new EntityFrameworkServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, ClientNoSqlDBLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<ClientNoSqlDBOptionsExtension>>()
                .TryAdd<IDatabase>(p => p.GetService<ClientNoSqlDBDatabase>())
                .TryAdd<IDbContextTransactionManager, ClientNoSqlDBTransactionManager>()
                .TryAdd<IDatabaseCreator, ClientNoSqlDBDatabaseCreator>()
                .TryAdd<IQueryContextFactory, ClientNoSqlDBQueryContextFactory>()
                .TryAdd<IProviderConventionSetBuilder, ClientNoSqlDBConventionSetBuilder>()
                .TryAdd<ITypeMappingSource, ClientNoSqlDBTypeMappingSource>()

                //// New Query pipeline
                .TryAdd<IShapedQueryCompilingExpressionVisitorFactory, ClientNoSqlDBShapedQueryCompilingExpressionVisitorFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, ClientNoSqlDBQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, ClientNoSqlDBQueryTranslationPostprocessorFactory>();



            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
