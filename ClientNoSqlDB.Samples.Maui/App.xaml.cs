using Caliburn.Micro;
using ClientNoSqlDB.Samples.Maui.Services;
using ClientNoSqlDB.Samples.Maui.ViewModels;
using System.ComponentModel;

namespace ClientNoSqlDB.Samples.Maui
{
    public partial class App : Caliburn.Micro.Maui.MauiApplication
    {
        private SimpleContainer container;
        public App()
        {
            InitializeComponent();

            Initialize();

            DisplayRootViewForAsync<MainViewModel>();

            //container = new SimpleContainer();

            //container.Instance(container);
            //container.Singleton<IDataService,DataService>();
        }

    }
}