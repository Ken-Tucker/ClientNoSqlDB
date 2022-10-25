using System;
using ClientNoSqlDB.Serialization;
using ClientNoSqlDB;
using Xunit;

namespace ClientNoSql.Tests
{

    public class DbTests3 : IDisposable
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DbTests3()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            PurgeDb();
        }

        DbInstance db;
        DbTable<AData> table;

        DbInstance Prepare()
        {
            db = new DbInstance("MyDatabase3");
            db.Map<AData, PrototypeBasedData>().Automap(i => i.Id, true);
            db.Initialize();
            return db;
        }

        private void PurgeDb()
        {
            using (var i = Prepare())
                i.Purge();

            db = Prepare();
            table = db.Table<AData>();
        }

        private void CleanUp()
        {
            db.Purge();
            db.Dispose();
        }

        [Fact]
        public void Indexing3()
        {
            db = new DbInstance(@"MyDatabase3\Indexing");

            db.Map<AData, PrototypeBasedData>().Automap(i => i.Id, true)
              .WithIndex("LastName", i => i.Name, StringComparer.CurrentCulture)
              .WithIndex("LastNameText", i => i.Name, StringComparer.CurrentCultureIgnoreCase);
            db.Initialize();

            var table = db.Table<AData>();
            table.Purge();

            db.BulkWrite(() =>
            {
                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new PrototypeBasedData { Name = "Test" + i });

                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new PrototypeBasedData { Name = "TeST" + i });
            });

            var list1count = table.IndexQueryByKey("LastName", "Test5").Count();
            var list2count = table.IndexQueryByKey("LastNameText", "TEst5").Count();

            Assert.Equal(100,list1count);
            Assert.Equal(200,list2count);
        }

        [Fact]
        public void LoadData3()
        {
            var table = db.Table<AData>();
            var items = table.LoadAll();
            Assert.NotNull(items);
        }

        [Fact]
        public void SaveData3()
        {
            db.BulkWrite(() =>
            {
                table.Purge();
                var key = 1;
                var newObj = new PrototypeBasedData { Id = key, Name = "test" };
                table.Save(newObj);

                var obj = table.LoadByKey(key);

                Assert.Equal(newObj.Name, obj.Name);

            });
        }

        public class Person
        {
            public int PersonID;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string Forename, Surname;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        }

        [Fact]
        public void TestGordon()
        {
            db = new DbInstance("gordon.db");
            db.Map<Person>().Automap(i => i.PersonID, true).WithIndex("Surname", i => i.Surname);
            db.Initialize();

            var table = db.Table<Person>();

            table.Purge();

            Person newPerson1 = new Person { Forename = "Joe", Surname = "Bloggs" };
            Person newPerson2 = new Person { Forename = "James", Surname = "Smith" };
            Person newPerson3 = new Person { Forename = "David", Surname = "Peterson" };
            Person newPerson4 = new Person { Forename = "Steve", Surname = "Gordon" };
            Person newPerson5 = new Person { Forename = "David", Surname = "Gordon" };
            Person newPerson6 = new Person { Forename = "Colin", Surname = "Gordon" };
            Person newPerson7 = new Person { Forename = "Michael", Surname = "Gordon" };

            var newPeople = new[]
            {
        newPerson1,
        newPerson2,
        newPerson3,
        newPerson4,
        newPerson5,
        newPerson6,
        newPerson7
      };

            table.Save(newPeople);

            var index = table.IndexQuery<string>("Surname");

            var queryIndex = index.GreaterThan("H", true).LessThan("T", true).ToLazyList();
            Assert.Equal(2, queryIndex.Count);
        }

        public void Dispose()
        {
            CleanUp();
        }
    }

    public struct Point
    {
        public int X, Y;
    }

    public class PointSerializer
    {
        public static Point ReadPoint(DataReader reader)
        {
            return new Point { X = reader.ReadInt32(), Y = reader.ReadInt32() };
        }

        public static void WritePoint(DataWriter writer, Point point)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }
    }
}
