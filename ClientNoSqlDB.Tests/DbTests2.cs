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

        private void PurgeDb()
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

        private void CleanUp()
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

            var localTable = db.Table<IData>();
            localTable.Purge();

            db.BulkWrite(() =>
            {
                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        localTable.Save(new InterfaceBasedData { Name = "Test" + i });

                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        localTable.Save(new InterfaceBasedData { Name = "TeST" + i });
            });

            var list1count = localTable.IndexQuery<string>("LastName").Key("Test5").Count();
            var list2count = localTable.IndexQuery<string>("LastNameText").Key("TEst5").Count();

            Assert.Equal(100, list1count);
            Assert.Equal(200, list2count);

            var list3count = localTable.IndexQuery<string>("LastName").GreaterThan("Test5").Count();
            var list4count = localTable.IndexQuery<string>("LastName").LessThan("Test6").Count();
            var list5count = localTable.IndexQuery<string>("LastName").LessThan("Test6").GreaterThan("Test5").Count();

            Assert.Equal(900, list3count);
            Assert.Equal(1200, list4count);
            Assert.Equal(100, list5count);

            var list6count = localTable.IndexQuery<string>("LastName").GreaterThan("Test5", true).Count();
            var list7count = localTable.IndexQuery<string>("LastName").LessThan("Test6", true).Count();
            var list8count = localTable.IndexQuery<string>("LastName").LessThan("Test6", true).GreaterThan("Test5", true).Count();

            Assert.Equal(1000, list6count);
            Assert.Equal(1300, list7count);
            Assert.Equal(300, list8count);
        }

        [Fact]
        public void LoadData2()
        {
            var localTable = db.Table<IData>();
            var items = localTable.LoadAll();
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
