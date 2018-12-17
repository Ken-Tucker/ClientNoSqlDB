using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace ClientNoSqlDB.Samples.Ios
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            table = new UITableView(View.Bounds); // defaults to Plain style
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
            string[] tableItems = names.ToArray();
            table.Source = new TableSource(tableItems);
            Add(table);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }

    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
