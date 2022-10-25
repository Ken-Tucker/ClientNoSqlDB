using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientNoSqlDB;
using ClientNoSqlDB.Serialization;
using Xunit;

namespace ClientNoSql.Tests
{
    public class DbTests2 : WorkItemTest, IDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DbTests2()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            PurgeDb();
        }

        DbInstance db;
        DbTable<IData> table;

        DbInstance Prepare()
        {
            db = new DbInstance("MyDatabase2");
            db.Map<IData, InterfaceBasedData>().Automap(i => i.Id, true);
            db.Initialize();
            return db;
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void PurgeDb()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            try
            {
                Extender.RegisterType<Point, PointSerializer>(2000);
            }
            catch { }

            using (var i = Prepare())
                i.Purge();

            db = Prepare();
            table = db.Table<IData>();
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void CleanUp()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            db.Purge();
            db.Dispose();
        }

        [Fact]
        public void Indexing2()
        {
            db = new DbInstance(@"MyDatabase2\Indexing");

            db.Map<IData, InterfaceBasedData>().Automap(i => i.Id, true)
              .WithIndex("LastName", i => i.Name, StringComparer.CurrentCulture)
              .WithIndex("LastNameText", i => i.Name, StringComparer.CurrentCultureIgnoreCase);
            db.Initialize();

            var table = db.Table<IData>();
            table.Purge();

            db.BulkWrite(() =>
            {
                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new InterfaceBasedData { Name = "Test" + i });

                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new InterfaceBasedData { Name = "TeST" + i });
            });
            
            var list1count = table.IndexQuery<string>("LastName").Key("Test5").Count();
            var list2count = table.IndexQuery<string>("LastNameText").Key("TEst5").Count();

            Assert.Equal(100, list1count);
            Assert.Equal(200,list2count);

            var list3count = table.IndexQuery<string>("LastName").GreaterThan("Test5").Count();
            var list4count = table.IndexQuery<string>("LastName").LessThan("Test6").Count();
            var list5count = table.IndexQuery<string>("LastName").LessThan("Test6").GreaterThan("Test5").Count();

            Assert.Equal(900, list3count);
            Assert.Equal(1200, list4count);
            Assert.Equal(100, list5count);

            var list6count = table.IndexQuery<string>("LastName").GreaterThan("Test5", true).Count();
            var list7count = table.IndexQuery<string>("LastName").LessThan("Test6", true).Count();
            var list8count = table.IndexQuery<string>("LastName").LessThan("Test6", true).GreaterThan("Test5", true).Count();

            Assert.Equal(1000, list6count);
            Assert.Equal(1300, list7count);
            Assert.Equal(300, list8count);
        }

        [Fact]
        public void LoadData2()
        {
            var table = db.Table<IData>();
            var items = table.LoadAll();
            Assert.NotNull(items);
        }

        [Fact]
        public void SaveData2()
        {
            db.BulkWrite(() =>
          {
              table.Purge();
              var key = 1;
              var newObj = new InterfaceBasedData { Id = key, Name = "test" };
              table.Save(newObj);

              var obj = table.LoadByKey(key);

              Assert.Equal(newObj.Name, obj.Name);

          });
        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
