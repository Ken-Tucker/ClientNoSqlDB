using ClientNoSqlDB.Samples.NetCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientNoSqlDB.Samples.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> list;
            using (var db = new PersonDBContext("testing"))
            {
                db.Initialize();
                list = db.People.ToList();
                if (list != null && !list.Any())
                {
                    db.People.AddRange(new Person() { FirstName = "Ken", Id = 1, LastName = "Tucker" },
                    new Person() { FirstName = "Tony", Id = 2, LastName = "Stark" },
                    new Person() { FirstName = "John", Id = 3, LastName = "Papa" },
                    new Person() { FirstName = "Delete", Id = 4, LastName = "Me" });
                }
                db.SaveChanges();
                var item = db.People.FirstOrDefault(person => person.Id == 4);
                if (item != null)
                {
                    db.People.Remove(item);
                }

                foreach (var p in list  ?? Enumerable.Empty<Person>())
                {
                    Console.WriteLine($"{p.FirstName} {p.LastName}");
                }

            }


        }
    }

    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}

