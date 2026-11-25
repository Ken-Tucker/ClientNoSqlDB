using ClientNoSqlDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNoSqlDB.Samples.NetCore.Data
{
    public class PersonDBContext(string path) : ClientNoSqlDbContext(path)
    {
        public DbSet<Person> People => Set<Person>();
    }
}
