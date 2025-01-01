using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBConventionSetBuilder : ProviderConventionSetBuilder
    {
        public ClientNoSqlDBConventionSetBuilder(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
        {
        }
    }
}
