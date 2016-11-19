using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Security;
using SOA_Android.Activities;
using SOA_Android.Support_Classes;

namespace SOA_Android.Activities
{
    [Activity]
    public class MainMenuActivity : Activity
    {
        public Button btnHourColor;
        public ColorConfiguration colors = ColorConversion.GetColorSetupFromXML();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.MainMenu);
            var btnRead = (Button)FindViewById(Resource.Id.btnRead);
            btnRead.Click += btnRead_Click;
            var btnWrite = (Button) FindViewById(Resource.Id.btnWrite);
            btnWrite.Click += btnWrite_Click;
            var btnConfig = (Button)FindViewById(Resource.Id.btnConfig);
            btnConfig.Click += btnConfig_Click;

            btnHourColor = (Button)FindViewById(Resource.Id.btnHourColor);

            var timer = new Timer {Interval = 1000};
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            timer.Start();
        }

        public override void OnBackPressed()
        { }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var secondsElapsedFromToday = (DateTime.Now - DateTime.Today).TotalSeconds;
            btnHourColor.SetBackgroundColor(ColorConversion.GetGradientColor(secondsElapsedFromToday, colors.HourColor.LowColor, colors.HourColor.HighColor, double.Parse(ColorConversion.DefaultColors.HourColor.HighColor.Threshold)).ToAndroidColor());
        }

        protected void btnRead_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ReadActivity));
        }

        protected void btnWrite_Click(object sender, EventArgs e)
        {
            var sensorDialog = new AlertDialog.Builder(this);
            sensorDialog.SetTitle("¿Qué sensor desea sensar?");
            sensorDialog.SetNegativeButton("Proximidad", btnProximity_Click);
            sensorDialog.SetNeutralButton("GPS", btnGPS_Click);
            sensorDialog.SetPositiveButton("Ninguno", delegate { sensorDialog.Dispose(); });
            sensorDialog.Show();
        }

        protected void btnProximity_Click(object sender, EventArgs e)
        {
            var proximityIntent = new Intent(this, typeof(WriteActivity));
            proximityIntent.PutExtra("SensorType", "Proximidad");
            StartActivity(proximityIntent);
        }

        protected void btnGPS_Click(object sender, EventArgs e)
        {
            var gpsIntent = new Intent(this, typeof(WriteActivity));
            gpsIntent.PutExtra("SensorType", "GPS");
            StartActivity(gpsIntent);
        }

        protected void btnConfig_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ConfigActivity));
        }
    }
}