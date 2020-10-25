# ClientNoSqlDB
Client NoSql database written in a .net standard 2.0 library 

It will work with .net Framework 4.6.1, Xamarin.Android, Xamarin.iOS, and .net core.  


This NoSql database is based on Lex.DB




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
