using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace SOA_Android.Activities
{
    [Activity(MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Splash);

            Task.Run(() =>
            {
                Thread.Sleep(4000);
                StartActivity(typeof(MainMenuActivity));
            });
        }

        public override void OnBackPressed()
        { }
    }
}