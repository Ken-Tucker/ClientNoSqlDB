using Caliburn.Micro;
using ClientNoSqlDB.Samples.Maui.Services;

namespace ClientNoSqlDB.Samples.Maui.ViewModels
{
    public class MainViewModel : Caliburn.Micro.Screen
    {
        IDataService dataService;

        public MainViewModel()
        {

            dataService = new DataService();
            People = dataService.GetPeopleFromDB();
        }

        private BindableCollection<Person> people;
        public BindableCollection<Person> People
        {
            get
            {
                return people;
            }
            set
            {
                people = value;
                NotifyOfPropertyChange(() => People);
            }
        }

    }
}
