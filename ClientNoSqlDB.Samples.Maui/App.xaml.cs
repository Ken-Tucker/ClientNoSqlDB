using Caliburn.Micro;
using ClientNoSqlDB.Samples.Maui.Services;
using ClientNoSqlDB.Samples.Maui.ViewModels;
using System.ComponentModel;

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