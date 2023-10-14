using Caliburn.Micro;

namespace ClientNoSqlDB.Samples.Maui.Services
{
    public class DataService : IDataService
    {
        public BindableCollection<Person> GetPeopleFromDB()
        {
            BindableCollection<Person> people;
            using (var db = new ClientNoSqlDB.DbInstance("testing"))
            {
                db.Map<Person>().Automap(i => i.Id);
                db.Initialize();
                people = ConvertToBindableCollection(db.LoadAll<Person>().ToList());
                if (people != null && !people.Any())
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
                people = ConvertToBindableCollection(db.LoadAll<Person>().ToList());
                return people;
            }
        }

        private BindableCollection<Person> ConvertToBindableCollection(List<Person> people)
        {
            BindableCollection<Person> bindablePeople = new BindableCollection<Person>();
            foreach (var p in people)
            {
                bindablePeople.Add(p);
            }
            return bindablePeople;
        }
    }

}
