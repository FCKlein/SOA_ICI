using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SOA_Android.Support_Classes;

namespace SOA_Android.Services
{
    [Service]
    public class ProximitySensorService : Service, ISensorEventListener
    {
        private Thread _thread;

        public static SensorManager SensorManager;
        public static Sensor ProximitySensor;

        private NameValueCollection _sensorDataToSend = null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            BeginWriting();
            return StartCommandResult.Sticky;
        }

        private void BeginWriting()
        {
            _thread = new Thread(() =>
            {
                while (_thread.IsAlive)
                {
                    if (_sensorDataToSend != null)
                    {
                        var i = new Intent();
                        i.SetAction("AndroidReading");
                        i.PutExtra("Value", _sensorDataToSend["value"]);

                        SendBroadcast(i);

                        var dweetResponse = Http.Post(Constants.DweetPostHttp, _sensorDataToSend);
                        
                        _sensorDataToSend = null;
                    }
                    Thread.Sleep(1500);
                }
                return;
            });
            _thread.Start();
        }
        
        private void InitializeProximitySensor()
        {
            SensorManager = (SensorManager)GetSystemService(SensorService);
            ProximitySensor = SensorManager.GetDefaultSensor(SensorType.Proximity);
            SensorManager.RegisterListener(this, ProximitySensor, SensorDelay.Normal);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            InitializeProximitySensor();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        
        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type != SensorType.Proximity) return;
            _sensorDataToSend = new NameValueCollection
            {
                { "type", "Proximity" },
                { "value", e.Values[0].ToString(CultureInfo.InvariantCulture) }
            };
        }

        #region Not-used interface methods

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }
        #endregion
    }
}