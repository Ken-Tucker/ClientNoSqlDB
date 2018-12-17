using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;

namespace ClientNoSqlDB.Samples.Droid
{
    [Activity(Label = "BasicTable", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        string[] items;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            List<Person> list;
            using (var db = new ClientNoSqlDB.DbInstance("testing"))
            {
                db.Map<Person>().Automap(i => i.Id);
                db.Initialize();
                list = db.LoadAll<Person>().ToList();
                if (list != null && !list.Any())
                {
                    db.Save(new Person() { FirstName = "Ken", Id = 1, LastName = "Tucker" },
                    new Person() { FirstName = "Tony", Id = 2, LastName = "Stark" },
                    new Person() { FirstName = "John", Id = 3, LastName = "Papa" },
                    new Person() { FirstName = "Delete", Id = 4, LastName = "Me" });
                }

                var item = db.LoadByKey<Person>(4);
                if (item != null)
                {
                    db.Delete<Person>(item);
                }
                list = db.LoadAll<Person>().ToList();
            }
            List<string> names = new List<string>();
            foreach (var p in list)
            {
                names.Add($"{p.FirstName} {p.LastName}");
            }
            items = names.ToArray();
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Main);
        }
    }

    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}

