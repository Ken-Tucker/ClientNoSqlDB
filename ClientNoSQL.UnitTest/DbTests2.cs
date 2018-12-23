using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientNoSqlDB;
using ClientNoSqlDB.Serialization;
using NUnit.Framework;

namespace ClientNoSql.Tests
{
  [TestFixture]
  public class DbTests2 : WorkItemTest
  {


    DbInstance db;
    DbTable<IData> table;

    DbInstance Prepare()
    {
      db = new DbInstance("MyDatabase2");
      db.Map<IData, InterfaceBasedData>().Automap(i => i.Id, true);
      db.Initialize();
      return db;
    }

    [OneTimeSetUpAttribute]
    public void PurgeDb()
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

    [OneTimeTearDownAttribute]
    public void CleanUp()
    {
      db.Purge();
      db.Dispose();
    }

    [Test]
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

      Assert.AreEqual(list1count, 100);
      Assert.AreEqual(list2count, 200);

      var list3count = table.IndexQuery<string>("LastName").GreaterThan("Test5").Count();
      var list4count = table.IndexQuery<string>("LastName").LessThan("Test6").Count();
      var list5count = table.IndexQuery<string>("LastName").LessThan("Test6").GreaterThan("Test5").Count();

      Assert.AreEqual(900, list3count);
      Assert.AreEqual(1200, list4count);
      Assert.AreEqual(100, list5count);

      var list6count = table.IndexQuery<string>("LastName").GreaterThan("Test5", true).Count();
      var list7count = table.IndexQuery<string>("LastName").LessThan("Test6", true).Count();
      var list8count = table.IndexQuery<string>("LastName").LessThan("Test6", true).GreaterThan("Test5", true).Count();

      Assert.AreEqual(1000, list6count);
      Assert.AreEqual(1300, list7count);
      Assert.AreEqual(300, list8count);
    }

    [Test]
    public void LoadData2()
    {
      var table = db.Table<IData>();
      var items = table.LoadAll();
    }

    [Test]
    public void SaveData2()
    {
      var swatch = DateTime.Now;

      db.BulkWrite(() =>
      {
        table.Purge();
        var key = 1;
        var newObj = new InterfaceBasedData { Id = key, Name = "test" };
        table.Save(newObj);

        var obj = table.LoadByKey(key);

        Assert.AreEqual(newObj.Name, obj.Name);

      });
    }
  }
}
