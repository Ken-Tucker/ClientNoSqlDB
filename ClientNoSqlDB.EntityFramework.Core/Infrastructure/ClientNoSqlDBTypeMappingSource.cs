using Microsoft.EntityFrameworkCore.Storage;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBTypeMappingSource : TypeMappingSource
    {
        public ClientNoSqlDBTypeMappingSource(TypeMappingSourceDependencies dependencies) : base(dependencies)
        {
        }
    }
}
