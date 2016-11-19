using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SOA_Android.Support_Classes;

namespace SOA_Android.Services
{
    [Service]
    public class DweetReaderService : Service
    {
        private Thread _thread;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            BeginReading();
            return StartCommandResult.Sticky;
        }

        private void BeginReading()
        {
            _thread = new Thread(() =>
            {
                while (_thread.IsAlive)
                {
                    var dweetResponse = Http.Get(Constants.DweetGetHttp);
                    if (dweetResponse == null) continue;
                    var i = new Intent();
                    i.SetAction("ArduinoReading");
                    i.PutExtra("SendingDateTime", dweetResponse.SendingDateTime.ToString(CultureInfo.InvariantCulture));
                    i.PutExtra("Type", dweetResponse.Type);
                    i.PutExtra("Value", dweetResponse.Value);

                    SendBroadcast(i);

                    Thread.Sleep(1500);
                }
                return;
            });
            _thread.Start();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        
    }
}