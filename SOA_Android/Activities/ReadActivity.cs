using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mono.Security.Interface;
using SOA_Android.Services;
using SOA_Android.Support_Classes;

namespace SOA_Android.Activities
{
    [Activity]
    public class ReadActivity : Activity
    {
        public readonly ColorConfiguration colorConfig = ColorConversion.GetColorSetupFromXML();

        public Intent DweetReader;
        public TextView txtTempValue, txtHumidValue;
        public Button btnTempValue, btnHumidValue;

        private bool connected = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Read);

            var dweetResponse = Http.Post(Constants.DweetPostColorsHttp, new NameValueCollection
            {
                { "type", "colors" },
                { "value", ColorConversion.GetXMLString() }
            });

            DweetReader = new Intent(this, typeof(DweetReaderService));
            StartService(DweetReader);

            var filter = new IntentFilter("ArduinoReading");
            RegisterReceiver(new MyBroadcastReceiver(), filter);

            txtTempValue = (TextView)FindViewById(Resource.Id.txtTempValue);
            txtHumidValue = (TextView)FindViewById(Resource.Id.txtHumidValue);

            btnTempValue = (Button)FindViewById(Resource.Id.btnTempValue);
            btnHumidValue = (Button)FindViewById(Resource.Id.btnHumidValue);

            btnTempValue.SetBackgroundColor(Color.Transparent);
            btnHumidValue.SetBackgroundColor(Color.Transparent);
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopService(DweetReader);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartService(DweetReader);
        }

        [IntentFilter(new[] { "ArduinoReading" })]
        class MyBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (!intent.Action.Equals("ArduinoReading")) return;
                IFormatProvider culture = new CultureInfo("en-US", true);
                var reading = new ArduinoSensorReading
                {
                    SendingDateTime = DateTime.ParseExact(intent.GetStringExtra("SendingDateTime"), "MM/dd/yyyy HH:mm:ss", culture),
                    Type = intent.GetStringExtra("Type"),
                    Value = intent.GetStringExtra("Value")
                };

                if (reading.Type != "temperature" && reading.Type != "humidity") return;

                var values = reading.Value.Split('|');

                var tempValue = values[0].Substring(0, 5).Replace(',', '.');
                var humidValue = (values.Length > 1) ? values[1].Substring(0, 5).Replace(',', '.') : "0";

                var tempColor = Color.Black;
                var humidColor = Color.Black;

                if (reading.Value.IndexOf("%") != -1 || reading.Value.IndexOf("°C") != -1) return;

                var activity = ((ReadActivity)context);

                if ((DateTime.Now - reading.SendingDateTime).Seconds <= 60)
                {
                    if (!activity.connected)
                    {
                        activity.connected = true;
                        var toast = Toast.MakeText(context, "Detectado dispositivo Arduino sensando",
                            ToastLength.Long);
                        var toastView = toast.View;
                        toastView.SetBackgroundColor(Color.DarkGreen);
                        toast.Show();
                    }
                    tempColor =
                                ColorConversion.GetGradientColor(
                                    double.Parse(tempValue, NumberStyles.Any, CultureInfo.InvariantCulture),
                                    activity.colorConfig.TemperatureColor.LowColor,
                                    activity.colorConfig.TemperatureColor.HighColor, double.Parse(ColorConversion.DefaultColors.TemperatureColor.HighColor.Threshold)).ToAndroidColor();
                    tempValue += "°C";
                    humidColor =
                                ColorConversion.GetGradientColor(
                                    double.Parse(humidValue, NumberStyles.Any, CultureInfo.InvariantCulture),
                                    activity.colorConfig.HumidityColor.LowColor,
                                    activity.colorConfig.HumidityColor.HighColor, double.Parse(ColorConversion.DefaultColors.HumidityColor.HighColor.Threshold)).ToAndroidColor();
                    humidValue += "%";
                }
                else
                {
                    if (activity.connected)
                    {
                        activity.connected = true;
                        var toast = Toast.MakeText(context, "No hay dispositivos Arduino sensando",
                            ToastLength.Long);
                        var toastView = toast.View;
                        toastView.SetBackgroundColor(Color.Red);
                        toast.Show();
                        reading.Value = "?????";

                        humidColor = tempColor = Color.Red;
                    }
                }

                activity.txtTempValue.Text = tempValue;
                activity.btnTempValue.SetBackgroundColor(tempColor);
                activity.txtHumidValue.Text = humidValue;
                activity.btnHumidValue.SetBackgroundColor(humidColor);
            }
        }
    }
}