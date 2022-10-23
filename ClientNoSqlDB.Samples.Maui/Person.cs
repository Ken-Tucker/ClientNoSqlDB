namespace ClientNoSqlDB.Samples.Maui
{
    public class Person : Caliburn.Micro.PropertyChangedBase
    {
        private string firstName;
        private int id;
        private string lastName;

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        } 
        public string LastName { get => lastName; 
            set 
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }
    }
}