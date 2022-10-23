using Foundation;

namespace ClientNoSqlDB.Samples.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : Caliburn.Micro.Maui.CaliburnApplicationDelegate
    {
        public AppDelegate()
        {
            Initialize();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}