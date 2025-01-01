using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClientNoSqlDB.EntityFramework.Core.Infrastructure
{
    public class ClientNoSqlDBTypeMapping : CoreTypeMapping
    {
        public ClientNoSqlDBTypeMapping(Type clrType,
        ValueComparer comparer = null,
        ValueComparer keyComparer = null,
        ValueComparer structuralComparer = null)
        : base(
            new CoreTypeMappingParameters(
                clrType,
                converter: null,
                comparer,
                keyComparer,
                structuralComparer,
                valueGeneratorFactory: null))
        {
        }

        private ClientNoSqlDBTypeMapping(CoreTypeMappingParameters parameters)
    : base(parameters)
        {
        }


        public override CoreTypeMapping WithComposedConverter(ValueConverter? converter, ValueComparer? comparer = null, ValueComparer? keyComparer = null, CoreTypeMapping? elementMapping = null, JsonValueReaderWriter? jsonValueReaderWriter = null)
        {
            throw new NotImplementedException();
        }

        protected override CoreTypeMapping Clone(CoreTypeMappingParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
