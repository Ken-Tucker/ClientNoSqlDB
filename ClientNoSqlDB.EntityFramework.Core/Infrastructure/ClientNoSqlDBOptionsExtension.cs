using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBOptionsExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtensionInfo Info => throw new NotImplementedException();

        public void ApplyServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public void Validate(IDbContextOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
