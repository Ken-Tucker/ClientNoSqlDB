using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNoSqlDB.Samples.Maui.Services
{
    public interface IDataService
    {
        BindableCollection<Person> GetPeopleFromDB();
    }
}
