using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SOA_Android.Services;
using SOA_Android.Support_Classes;

namespace SOA_Android.Activities
{
    [Activity]
    public class WriteActivity : Activity
    {
        private readonly ColorConfiguration colorConfig = ColorConversion.GetColorSetupFromXML();
        private string sensorType = "";

        private TextView txtSensorTitle, txtSensorValue;
        private Button btnSensorValue;
        private Intent currentSensorService = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Write);

            var dweetResponse = Http.Post(Constants.DweetPostColorsHttp, new NameValueCollection
            {
                { "type", "colors" },
                { "value", ColorConversion.GetXMLString() }
            });

            sensorType = Intent.GetStringExtra("SensorType") ?? "?????";

            txtSensorTitle = (TextView) FindViewById(Resource.Id.txtSensorTitle);
            txtSensorTitle.Text = "Sensando " + sensorType;

            txtSensorValue = (TextView)FindViewById(Resource.Id.txtSensorValue);
            txtSensorValue.Text = "";

            btnSensorValue = (Button)FindViewById(Resource.Id.btnSensorValue);
            btnSensorValue.SetBackgroundColor(Color.Transparent);

            var filter = new IntentFilter("AndroidReading");
            RegisterReceiver(new MyBroadcastReceiver(), filter);

            switch (sensorType)
            {
                case "Proximidad":
                    currentSensorService = new Intent(this, typeof(ProximitySensorService));
                    btnSensorValue.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                    break;
                case "GPS":
                    currentSensorService = new Intent(this, typeof(GPSService));
                    txtSensorValue.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                    btnSensorValue.Visibility = ViewStates.Gone;
                    break;
            }

            if(currentSensorService != null)
                StartService(currentSensorService);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (currentSensorService != null)
                StopService(currentSensorService);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (currentSensorService != null)
                StartService(currentSensorService);
        }

        [IntentFilter(new[] { "AndroidReading" })]
        class MyBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (!intent.Action.Equals("AndroidReading")) return;

                var reading = intent.GetStringExtra("Value");

                var activity = ((WriteActivity)context);

                if (reading.Equals("NOTHING"))
                    activity.StartActivity(typeof(MainMenuActivity));

                switch (activity.sensorType)
                {
                    case "Proximidad":
                        double value = 0;
                        if (!double.TryParse(reading, out value)) return;
                        var color =
                                ColorConversion.GetGradientColor(
                                    double.Parse(reading, NumberStyles.Any, CultureInfo.InvariantCulture),
                                    activity.colorConfig.ProximityColor.LowColor,
                                    activity.colorConfig.ProximityColor.HighColor, double.Parse(ColorConversion.DefaultColors.ProximityColor.HighColor.Threshold)).ToAndroidColor();
                        activity.btnSensorValue.SetBackgroundColor(color);
                        break;
                    case "GPS":
                        var splits = reading.Split('|');
                        reading = $"Latitud: {splits[0].Substring(0,4)}\nLongitud: {splits[1].Substring(0, 4)}";
                        break;
                }

                activity.txtSensorValue.Text = reading;
            }
        }
    }
}