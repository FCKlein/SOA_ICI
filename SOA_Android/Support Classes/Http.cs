using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace SOA_Android.Support_Classes
{
    public static class Http
    {
        public static ArduinoSensorReading Get(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            using (var response = request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var json = sr.ReadToEnd();
                    try
                    {
                        var deserializedData = (LatestDweet)JsonConvert.DeserializeObject(json, typeof(LatestDweet));
                        return new ArduinoSensorReading
                        {
                            SendingDateTime = DateTime.Parse(deserializedData.With.First().Created),
                            Type = deserializedData.With.First().Content.Type,
                            Value = deserializedData.With.First().Content.Value
                        };
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            using (var client = new WebClient())
            {
                return client.UploadValues(uri, pairs);
            }
        }
    }
}