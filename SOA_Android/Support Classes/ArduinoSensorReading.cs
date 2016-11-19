using System;

namespace SOA_Android.Support_Classes
{
    public class ArduinoSensorReading
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime SendingDateTime { get; set; }
    }
}