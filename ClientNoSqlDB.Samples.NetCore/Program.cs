﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientNoSqlDB.Samples.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
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

                foreach(var p in list)
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

