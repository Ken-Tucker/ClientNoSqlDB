using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Diagnostics;
using ClientNoSqlDB;
using Xunit;

namespace ClientNoSql.Tests
{

    public class AsynchronousAttribute : Attribute { }

    public class WorkItemTest
    {
        public void TestComplete() { }
    }

    public class DbTests : WorkItemTest, IDisposable
    {
        public DbTests()
        {
            db = Prepare();
            table = db.Table<MyData>();
            PurgeDb();
        }

        DbInstance db;
        DbTable<MyData> table;

        DbInstance Prepare()
        {
            db = new DbInstance("MyDatabase");
            db.Map<MyData>().Automap(i => i.Id, true);
            db.Initialize();
            return db;
        }

        [Fact]
        public void TestPKTypes()
        {
            TestPKKey(i => i.KeyBool, (o, v) => o.KeyBool = v, true);
            TestPKKey(i => i.KeyBool, (o, v) => o.KeyBool = v, false);

            TestPKKey(i => i.KeyBoolN, (o, v) => o.KeyBoolN = v, null);
            TestPKKey(i => i.KeyBoolN, (o, v) => o.KeyBoolN = v, true);
            TestPKKey(i => i.KeyBoolN, (o, v) => o.KeyBoolN = v, false);

            TestPKKey(i => i.KeyGuid, (o, v) => o.KeyGuid = v, Guid.NewGuid());
            TestPKKey(i => i.KeyGuidN, (o, v) => o.KeyGuidN = v, Guid.NewGuid());
            TestPKKey(i => i.KeyGuidN, (o, v) => o.KeyGuidN = v, null);



        }
        
        private void TestPKKey<T>(Expression<Func<MyDataKeys, T>> pkGetter, Action<MyDataKeys, T> pkSetter, T key)
        {
            db = new DbInstance("DbKeys");
            db.Map<MyDataKeys>().Key(pkGetter);
            db.Initialize();
            var getter = pkGetter.Compile();
            var obj1 = new MyDataKeys();
            pkSetter(obj1, key);
            db.Save(obj1);

            var obj2 = db.LoadByKey<MyDataKeys>(key);

            Assert.Equal(getter(obj1), getter(obj2));

            db.Purge();
        }


        private void PurgeDb()
        {
            using (var i = Prepare())
                i.Purge();

            db = Prepare();
            table = db.Table<MyData>();
        }

        private void CleanUp()
        {
            Debugger.Break();
            db.Purge();
            db.Dispose();
        }
        

        private void OpenDb()
        {
            db = new DbInstance("My Database");
            db.Initialize();
        }

        
        private void OpenDbComplexPath()
        {
            db = new DbInstance(@"My Database\My Schema");
            db.Initialize();
        }


        [Fact]
        public void DoubleOpenDbComplexPath()
        {
            try
            {
                db = new DbInstance(@"My Database\My Schema");
                db.Initialize();
                db.Initialize();

                Assert.True(false, "InvalidOperationException expected");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        //    [ExpectedException(typeof(InvalidOperationException))]
        public void MapDb()
        {
            try
            {
                db = new DbInstance(@"My Database\My Schema");
                db.Map<MyData>();

                db.Initialize();


                Assert.True(false, "InvalidOperationException expected");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        //    [ExpectedException(typeof(InvalidOperationException))]
        public void MapDbWrong()
        {
            try
            {
                db = new DbInstance(@"My Database\My Schema");

                db.Initialize();

                db.Map<MyData>().Automap(i => i.Id);

                Assert.True(false, "InvalidOperationException expected");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        public void Indexing()
        {
            db = new DbInstance(@"My Database\Indexing");

            db.Map<MyData>().Automap(i => i.Id, true)
              .WithIndex("LastName", i => i.Name, StringComparer.CurrentCulture)
              .WithIndex("LastNameText", i => i.Name, StringComparer.CurrentCultureIgnoreCase);
            db.Initialize();

            var table = db.Table<MyData>();
            table.Purge();

            db.BulkWrite(() =>
            {
                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new MyData { Name = "Test" + i });

                for (var s = 0; s < 100; s++)
                    for (var i = 0; i < 10; i++)
                        table.Save(new MyData { Name = "TeST" + i });
            });

            var list1count = table.IndexQueryByKey("LastName", "Test5").Count();
            var list2count = table.IndexQueryByKey("LastNameText", "TEst5").Count();

            Assert.Equal(100,list1count);
            Assert.Equal(200,list2count);
        }

        [Fact]
        public void IndexingDetails()
        {
            db = new DbInstance(@"My Database\Indexing2");

            db.Map<MyData>().Automap(i => i.Id, true).WithIndex("Test", i => i.IntField);
            db.Initialize();

            var table = db.Table<MyData>();
            table.Purge();

            db.BulkWrite(() =>
            {
                table.Save(new MyData { IntField = 1 });
                table.Save(new MyData { IntField = 1 });
                table.Save(new MyData { IntField = 1 });
                table.Save(new MyData { IntField = 1 });
                table.Save(new MyData { IntField = 1 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 3 });
                table.Save(new MyData { IntField = 3 });
                table.Save(new MyData { IntField = 3 });
                table.Save(new MyData { IntField = 3 });
                table.Save(new MyData { IntField = 3 });
                table.Save(new MyData { IntField = 4 });
                table.Save(new MyData { IntField = 5 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 6 });
                table.Save(new MyData { IntField = 7 });
                table.Save(new MyData { IntField = 8 });
                table.Save(new MyData { IntField = 8 });
                table.Save(new MyData { IntField = 8 });
                table.Save(new MyData { IntField = 8 });
                table.Save(new MyData { IntField = 8 });
                table.Save(new MyData { IntField = 9 });
            });

            var list1 = table.LoadAll();

            var index = table.IndexQuery<int>("Test");

            Assert.Equal(index.Key(1).Count(), list1.Count(i => i.IntField == 1));
            Assert.Equal(index.Key(8).Count(), list1.Count(i => i.IntField == 8));

            Assert.Equal(index.GreaterThan(6, true).LessThan(8).Count(), list1.Count(i => i.IntField >= 6 && i.IntField < 8));

            IdSequenceEqual(index.GreaterThan(6).LessThan(8).ToList(), list1.Where(i => i.IntField > 6 && i.IntField < 8));
            IdSequenceEqual(index.LessThan(8).ToList(), list1.Where(i => i.IntField < 8));
            IdSequenceEqual(index.GreaterThan(6, true).ToList(), list1.Where(i => i.IntField >= 6));
            IdSequenceEqual(index.GreaterThan(7, true).LessThan(7).ToList(), list1.Where(i => i.IntField >= 7 && i.IntField < 7));
            IdSequenceEqual(index.GreaterThan(7).LessThan(7, true).ToList(), list1.Where(i => i.IntField > 7 && i.IntField <= 7));
        }

        static void IdSequenceEqual(IEnumerable<MyData> a, IEnumerable<MyData> b)
        {
            Assert.True(a.OrderBy(i => i.Id).Select(i => i.Id).SequenceEqual(b.OrderBy(i => i.Id).Select(i => i.Id)));
        }


        private void LoadData()
        {
            var table = db.Table<MyData>();
            var items = table.LoadAll();
        }

        [Fact]
        public void SaveData()
        {
            var swatch = DateTime.Now;

            db.BulkWrite(() =>
            {
                table.Purge();
                var key = 1;
                var newObj = new MyData { Id = key, Name = "test" };
                table.Save(newObj);

                var obj = table.LoadByKey(key);

                Assert.Equal(newObj.Name, obj.Name);

            });
        }

        [Fact]
        public void SaveDataBulk()
        {
            db.Purge();
            db.BulkWrite(() =>
            {
                var cnt = DoSaveDataBulk();

                Assert.Equal(table.Count(), cnt);
            });
        }

        int DoSaveDataBulk()
        {
            table.Purge();
            var list = new List<MyData>();
            var cnt = 50000;
            for (int i = 0; i < cnt; i++)
                list.Add(new MyData { Name = "test " + i, LastName = "My Some Last Name " + i });

            table.Save(list);
            return cnt;
        }

        [Fact]
        public void LoadDataBulk()
        {
            db.BulkWrite(() =>
            {
                var cnt = DoSaveDataBulk();
                var load = table.LoadAll();
                Assert.Equal(cnt, load.Length);
            });
        }

        
        private void Compact()
        {
            table.Compact();
        }


        private void CheckInfo()
        {
            var info1 = table.GetInfo();
            var info2 = db.GetInfo();
        }


        [Fact]
        public void RoundTripNulls()
        {
            var obj = new MyData();

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.BoolNField, newObj.BoolNField);
            Assert.Equal(obj.IntNField, newObj.IntNField);
            Assert.Equal(obj.LongNField, newObj.LongNField);
            Assert.Equal(obj.DoubleNField, newObj.DoubleNField);
            Assert.Equal(obj.FloatNField, newObj.FloatNField);
            Assert.Equal(obj.DecimalNField, newObj.DecimalNField);
            Assert.Equal(obj.TimeSpanNField, newObj.TimeSpanNField);
            Assert.Equal(obj.DateTimeNField, newObj.DateTimeNField);
            Assert.Equal(obj.DateTimeOffsetNField, newObj.DateTimeOffsetNField);
            Assert.Equal(obj.GuidNField, newObj.GuidNField);
            Assert.Equal(obj.EnumNField, newObj.EnumNField);
            Assert.Equal(obj.Name, newObj.Name);

            var info = table.GetInfo();
            Assert.NotEqual(0, info.DataSize);
            Assert.NotEqual(0, info.IndexSize);
        }

        #region Bool RoundTrip Tests

        [Fact]
        public void RoundTripBool1()
        {
            var obj = new MyData { BoolField = true, BoolNField = false };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.BoolField, newObj.BoolField);
            Assert.Equal(obj.BoolNField, newObj.BoolNField);
        }

        [Fact]
        public void RoundTripBool2()
        {
            var obj = new MyData { BoolField = false, BoolNField = true };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.BoolField, newObj.BoolField);
            Assert.Equal(obj.BoolNField, newObj.BoolNField);
        }

        #endregion

        #region Int RoundTrip Tests

        [Fact]
        public void RoundTripInt1()
        {
            var obj = new MyData { IntField = int.MaxValue, IntNField = int.MinValue };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.IntField, newObj.IntField);
            Assert.Equal(obj.IntNField, newObj.IntNField);
        }

        [Fact]
        public void RoundTripInt2()
        {
            var obj = new MyData { IntField = int.MinValue, IntNField = int.MaxValue };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.IntField, newObj.IntField);
            Assert.Equal(obj.IntNField, newObj.IntNField);
        }

        #endregion

        #region Long RoundTrip Tests

        [Fact]
        public void RoundTripLong1()
        {
            var obj = new MyData { LongField = long.MaxValue, LongNField = long.MinValue };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.LongField, newObj.LongField);
            Assert.Equal(obj.LongNField, newObj.LongNField);
        }

        [Fact]
        public void RoundTripLong2()
        {
            var obj = new MyData { LongField = long.MinValue, LongNField = long.MaxValue };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.LongField, newObj.LongField);
            Assert.Equal(obj.LongNField, newObj.LongNField);
        }

        #endregion

        #region Float RoundTrip Tests

        [Fact]
        public void RoundTripFloat1()
        {
            var obj = new MyData { FloatField = (float)Math.PI, FloatNField = (float)-Math.PI };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.FloatField, newObj.FloatField);
            Assert.Equal(obj.FloatNField, newObj.FloatNField);
        }

        #endregion

        #region Double RoundTrip Tests

        [Fact]
        public void RoundTripDouble1()
        {
            var obj = new MyData { DoubleField = Math.PI, DoubleNField = -Math.PI };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.DoubleField, newObj.DoubleField);
            Assert.Equal(obj.DoubleNField, newObj.DoubleNField);
        }

        #endregion

        #region Decimal RoundTrip Tests

        [Fact]
        public void RoundTripDecimal1()
        {
            var obj = new MyData { DecimalField = (decimal)Math.PI, DecimalNField = (decimal)-Math.PI };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.DecimalField, newObj.DecimalField);
            Assert.Equal(obj.DecimalNField, newObj.DecimalNField);
        }

        #endregion

        #region String RoundTrip Tests

        [Fact]
        public void RoundTripString1()
        {
            var obj = new MyData { Name = "Test ABC" };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.Name, newObj.Name);
        }

        #endregion

        #region Guid RoundTrip Tests

        [Fact]
        public void RoundTripGuid1()
        {
            var obj = new MyData { GuidField = Guid.NewGuid(), GuidNField = Guid.NewGuid() };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.GuidField, newObj.GuidField);
            Assert.Equal(obj.GuidNField, newObj.GuidNField);
        }

        #endregion

        #region Enum RoundTrip Tests

        [Fact]
        public void RoundTripEnum1()
        {
            var obj = new MyData { EnumField = TestEnum.EnumValue1, EnumNField = TestEnum.EnumValue2 };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.EnumField, newObj.EnumField);
            Assert.Equal(obj.EnumNField, newObj.EnumNField);
        }

        #endregion

        #region TimeSpan RoundTrip Tests

        [Fact]
        public void RoundTripTimeSpan1()
        {
            var obj = new MyData { TimeSpanField = new TimeSpan(1, 2, 3), TimeSpanNField = new TimeSpan(2, 3, 4) };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.TimeSpanField, newObj.TimeSpanField);
            Assert.Equal(obj.TimeSpanNField, newObj.TimeSpanNField);
        }

        #endregion

        #region DateTime RoundTrip Tests

        [Fact]
        public void RoundTripDateTime1()
        {
            var obj = new MyData { DateTimeField = new DateTime(1, 2, 3, 4, 5, 6), DateTimeNField = new DateTime(2, 3, 4, 5, 6, 7) };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.DateTimeField, newObj.DateTimeField);
            Assert.Equal(obj.DateTimeNField, newObj.DateTimeNField);
        }

        #endregion

        #region DateTimeOffset RoundTrip Tests

        [Fact]
        public void RoundTripDateTimeOffset1()
        {
            var obj = new MyData { DateTimeOffsetField = new DateTimeOffset(1, 2, 3, 4, 5, 6, TimeSpan.FromMinutes(60)), DateTimeOffsetNField = new DateTimeOffset(2, 3, 4, 5, 6, 7, TimeSpan.FromMinutes(120)) };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.Equal(obj.DateTimeOffsetField, newObj.DateTimeOffsetField);
            Assert.Equal(obj.DateTimeOffsetNField, newObj.DateTimeOffsetNField);
        }

        #endregion

        #region Lists RoundTrip Tests

        [Fact]
        public void RoundTripLists1()
        {
            var obj = new MyData
            {
                ListField = new List<int> { 1, 2, 3, 4, 5 },

                DictField = new Dictionary<string, int> { { "test1", 111 }, { "test2", 222 }, { "test3", 333 } }
            };

            table.Save(obj);

            var newObj = table.LoadByKey(obj.Id);

            Assert.True(obj.ListField.SequenceEqual(newObj.ListField));

            Assert.True(obj.DictField.Keys.SequenceEqual(newObj.DictField.Keys));
            Assert.True(obj.DictField.Values.SequenceEqual(newObj.DictField.Values));
        }

        #endregion

        #region BugFixes

        #region github issue #9

        public class TemplateModel
        {
            public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string ForeignIds { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public int Type { get; set; }
        }

        [Fact]
        void TestDeleteBugfix()
        {
            var db = new DbInstance("test.fix1");

            //mapping done before init
            db.Map<TemplateModel>().Automap(i => i.Id, false).WithIndex<int>("Type", i => i.Type);

            db.Initialize();

            //testing this in a method
            db.Table<TemplateModel>().Save(new TemplateModel { Id = 66, Name = "test", Type = 3 });
            db.Table<TemplateModel>().Save(new TemplateModel { Id = 67, Name = "test2", Type = 3 });
            //The Type is 3 for both records 
            //The first indexQuery returns 2 records, OK!
            var indexQuery = db.Table<TemplateModel>().IndexQueryByKey<int>("Type", 3).ToList();

            db.Table<TemplateModel>().DeleteByKey<int>(67);

            //allItems returns 1 record, OK!
            var allItems = db.Table<TemplateModel>().LoadAll().ToList();

            //indexQuery2 returns 0 records, wrong!
            var indexQuery2 = db.Table<TemplateModel>().IndexQueryByKey<int>("Type", 3).ToList();

            Assert.Single(indexQuery2);
        }

        #endregion

        #region github issue #10

        [Fact]
        public void TestLoadByKeyObj()
        {
            var obj = new MyData { GuidField = Guid.NewGuid() };

            table.Save(obj);

            var newObj = table.LoadByKey((object)obj.Id);

            Assert.Equal(obj.GuidField, newObj.GuidField);
        }

        [Fact]
        public void TestDeleteByKeyObj()
        {
            var obj = new MyData { GuidField = Guid.NewGuid() };

            table.Save(obj);

            Assert.True(table.DeleteByKey((object)obj.Id));

        }

        public void Dispose()
        {
            CleanUp();
        }

        #endregion

        #endregion
    }
}
