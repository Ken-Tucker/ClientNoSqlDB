using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;

namespace ClientNoSql.Samples.Xamarin.Android
{
    [Activity(Label = "ClientNoSql.Samples.Xamarin.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            List<Person> list;
            using (var db = new ClientNoSqlDB.DbInstance("test"))
            {
                db.Map<Person>().Automap(i => i.Id);
                db.Initialize();
                list = db.LoadAll<Person>().ToList();
                if (list != null && !list.Any())
                {
                    db.Save(new Person() { FirstName = "Ken", Id = 1, LastName = "Tucker" },
                    new Person() { FirstName = "Tony", Id = 2, LastName = "Stark" },
                    new Person() { FirstName = "John", Id = 3, LastName = "Papa" });
                }
            }
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }

    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}

