using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClientNoSqlDB.EntityFramework.Core
{
    public interface IClientNoSqlDBSingletonOptions : ISingletonOptions
    {
        bool IsNullabilityCheckEnabled { get; }
    }
}
