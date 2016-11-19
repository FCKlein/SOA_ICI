using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using SOA_Android.Activities;
using SOA_Android.Support_Classes;

namespace SOA_Android.Services
{
    [Service]
    public class GPSService : Service, ILocationListener
    {
        private Thread _thread;

        private Location _currentLocation;
        private LocationManager _locationManager;
        private string _locationProvider;
        private string _currentCoordinates;

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

        private void InitializeGPS()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);

            var criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            var acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
            _currentLocation = _locationManager.GetLastKnownLocation(_locationProvider);
            _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 1, this);
            if (_currentLocation == null) return;
            _currentCoordinates = $"{_currentLocation.Latitude:f6}|{_currentLocation.Longitude:f6}";
            _sensorDataToSend = new NameValueCollection
            {
                { "type", "GPS" },
                { "value", _currentCoordinates }
            };
        }
        
        public override void OnCreate()
        {
            base.OnCreate();
            InitializeGPS();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null) return;
            _currentCoordinates = $"{_currentLocation.Latitude:f6},{_currentLocation.Longitude:f6}";
            _sensorDataToSend = new NameValueCollection
            {
                { "type", "GPS" },
                { "value", _currentCoordinates }
            };
        }

        #region Not-used interface methods

        public void OnProviderDisabled(string provider)
        {
            var mainIntent = new Intent(this, typeof(MainMenuActivity));
            mainIntent.AddFlags(ActivityFlags.NewTask);
            StopSelf();
            StartActivity(mainIntent);
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}