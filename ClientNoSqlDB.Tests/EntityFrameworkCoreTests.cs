using ClientNoSqlDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClientNoSql.Tests;

public class EntityFrameworkCoreTests
{
    [Fact]
    public async Task ContextLoadsQueriesAndPersistsTrackedChanges()
    {
        var path = "EntityFrameworkCore_" + Guid.NewGuid().ToString("N");

        using (var context = new RecordsContext(path))
        {
            context.Initialize();
            var first = new TestRecord { Name = "first" };
            context.Records.Add(first);
            context.Records.Add(new TestRecord { Name = "second" });
            Assert.Equal(2, await context.SaveChangesAsync(TestContext.Current.CancellationToken));
            Assert.NotEqual(0, first.Id);
            Assert.Equal(2, context.Records.Count());
        }

        using (var context = new RecordsContext(path))
        {
            context.Initialize();
            var record = context.Records.Single(item => item.Name == "first");
            record.Name = "updated";
            context.Records.Remove(context.Records.Single(item => item.Name == "second"));
            Assert.Equal(2, context.SaveChanges());
        }

        using (var context = new RecordsContext(path))
        {
            context.Initialize();
            Assert.Equal(new[] { "updated" }, context.Records.Select(item => item.Name).ToArray());
        }
    }

    private sealed class RecordsContext(string path) : ClientNoSqlDbContext(path)
    {
        public DbSet<TestRecord> Records => Set<TestRecord>();
    }

    public sealed class TestRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
