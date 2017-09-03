using Android.App;
using Android.Widget;
using Android.OS;

namespace ClientNoSql.Samples.XamarinForms
{
    [Activity(Label = "ClientNoSql.Samples.XamarinForms", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}

