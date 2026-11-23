# ClientNoSqlDB
Client NoSql database written in a .net standard 2.0 library 

It will work with .net Framework 4.6.2+, .net Maui, and .net 8.0+.  


This NoSql database is based on Lex.DB

 [![NuGet Package](https://img.shields.io/nuget/v/ClientNoSqlDB.svg?logo=nuget&logoColor=white&&style=for-the-badge&colorB=green)](https://www.nuget.org/packages/ClientNoSqlDB)



# You can contribute to the project in 3 ways

* Add a issue that reports a bug or requests a new feature

* Improve the documentation

* Issue a pull request that fixes a bug or adds a new feature.

If your pull request is accepted I will add you on as a contributer



# To use this library in your apps

Add the ClientNoSql nuget package to your app


To start off create an instance of the client no sql
 
     var db = new ClientNoSqlDB.DbInstance("testing")

Map the classes in your DB and initialize it.  Make sure you map your primary key.

                db.Map<Person>().Automap(i => i.Id);
                db.Initialize();
                
You can save items in the db like this

                   db.Save(new Person() { FirstName = "Ken", Id = 1, LastName = "Tucker" },
                    new Person() { FirstName = "Tony", Id = 2, LastName = "Stark" },
                    new Person() { FirstName = "John", Id = 3, LastName = "Papa" },
                    new Person() { FirstName = "Delete", Id = 4, LastName = "Me" });

Load a single item

               var item = db.LoadByKey<Person>(4);

Load all the items

               list = db.LoadAll<Person>().ToList();

Delete an item

                    db.Delete<Person>(item);

## Entity Framework Core

The `ClientNoSqlDB.EntityFrameworkCore` package provides a `DbContext` adapter for EF Core 10. Derive a context, expose `DbSet` properties, and call `Initialize` after construction. The context loads its mapped entities when initialized and writes tracked additions, updates, and deletions back to ClientNoSqlDB when `SaveChanges` is called.

```csharp
using ClientNoSqlDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class PeopleContext(string path) : ClientNoSqlDbContext(path)
{
    public DbSet<Person> People => Set<Person>();
}

using var db = new PeopleContext("people");
db.Initialize();
db.People.Add(new Person { Id = 1, FirstName = "Ada" });
db.SaveChanges();
var people = db.People.Where(person => person.FirstName == "Ada").ToList();
```

This adapter uses EF Core's in-memory query provider over the data loaded during `Initialize`; queries are evaluated in memory and see the context's snapshot. Entity types must have one primary key and a public parameterless constructor. Scalar mapped members must be public, read-write properties or mutable public fields; owned and inherited entity types are not supported.
