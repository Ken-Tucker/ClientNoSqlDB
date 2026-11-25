using Caliburn.Micro;
using ClientNoSqlDB.Samples.Maui.Data;
using Microsoft.EntityFrameworkCore;

namespace ClientNoSqlDB.Samples.Maui.Services
{
    public class DataService : IDataService
    {
        public BindableCollection<Person> GetPeopleFromDB()
        {
            BindableCollection<Person> people;
            using (var db = new PersonDBContext("testing"))
            {
                db.Initialize();
                people = ConvertToBindableCollection(db.People.ToList());
                if (people != null && !people.Any())
                {
                    db.People.AddRange(new Person() { FirstName = "Ken", Id = 1, LastName = "Tucker" },
                    new Person() { FirstName = "Tony", Id = 2, LastName = "Stark" },
                    new Person() { FirstName = "John", Id = 3, LastName = "Papa" },
                    new Person() { FirstName = "Delete", Id = 4, LastName = "Me" });
                }
                db.SaveChanges();
                var item = db.People.FirstOrDefault(person=>person.Id==4);
                if (item != null)
                {
                    db.People.Remove(item);
                }
                people = ConvertToBindableCollection(db.People.ToList());
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
