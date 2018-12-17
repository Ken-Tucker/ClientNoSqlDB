using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ClientNoSqlDB.XamarinForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ClientNoSqlDB.XamarinForms.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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

                list = db.LoadAll<Person>().ToList();


            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public class Person
        {
            public int Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

        }
    }
}
