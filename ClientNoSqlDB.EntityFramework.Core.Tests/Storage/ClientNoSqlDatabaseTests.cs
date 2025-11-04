using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Moq;
using System.Runtime.Serialization;
using Xunit;

namespace ClientNoSqlDB.EntityFramework.Core.Tests
{
    public class ClientNoSqlDBDatabaseTests
    {
        private static ClientNoSqlDB.EntityFramework.Core.Storage.ClientNoSqlDBDatabase CreateInstance()
        {
            // Create an uninitialized DatabaseDependencies to avoid running EF Core complex constructors
            var deps = (DatabaseDependencies)FormatterServices.GetUninitializedObject(typeof(DatabaseDependencies));

            // Use Moq for the interface dependencies
            var designTimeModel = new Mock<Microsoft.EntityFrameworkCore.Metadata.IDesignTimeModel>().Object;
            var updateAdapterFactory = new Mock<IUpdateAdapterFactory>().Object;

            return new ClientNoSqlDB.EntityFramework.Core.Storage.ClientNoSqlDBDatabase(deps, designTimeModel, updateAdapterFactory);
        }

        [Fact]
        public void SaveChanges_Throws_NotImplementedException()
        {
            var db = CreateInstance();
            var entries = new List<IUpdateEntry>();

            Assert.Throws<NotImplementedException>(() => db.SaveChanges(entries));
        }

        [Fact]
        public async Task SaveChangesAsync_Throws_NotImplementedException()
        {
            var db = CreateInstance();
            var entries = new List<IUpdateEntry>();

            await Assert.ThrowsAsync<NotImplementedException>(() => db.SaveChangesAsync(entries, CancellationToken.None));
        }
    }
}