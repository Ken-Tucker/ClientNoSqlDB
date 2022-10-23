using ClientNoSqlDB.Samples.Maui.ViewModels;

namespace ClientNoSqlDB.Samples.Maui
{
    public partial class App : Caliburn.Micro.Maui.MauiApplication
    {
        public App()
        {
            InitializeComponent();

            Initialize();

            DisplayRootViewForAsync<MainViewModel>();
        }
       
        
    }
}