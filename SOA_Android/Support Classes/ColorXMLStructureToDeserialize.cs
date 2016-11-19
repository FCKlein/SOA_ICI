using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SOA_Android.Support_Classes
{
    [XmlRoot(ElementName = "LowColor")]
    public class LowColor
    {
        [XmlElement(ElementName = "H")]
        public string H { get; set; }
        [XmlElement(ElementName = "S")]
        public string S { get; set; }
        [XmlElement(ElementName = "V")]
        public string V { get; set; }
        [XmlElement(ElementName = "Threshold")]
        public string Threshold { get; set; }
    }

    [XmlRoot(ElementName = "HighColor")]
    public class HighColor
    {
        [XmlElement(ElementName = "H")]
        public string H { get; set; }
        [XmlElement(ElementName = "S")]
        public string S { get; set; }
        [XmlElement(ElementName = "V")]
        public string V { get; set; }
        [XmlElement(ElementName = "Threshold")]
        public string Threshold { get; set; }
    }

    [XmlRoot(ElementName = "TemperatureColor")]
    public class TemperatureColor
    {
        [XmlElement(ElementName = "LowColor")]
        public LowColor LowColor { get; set; }
        [XmlElement(ElementName = "HighColor")]
        public HighColor HighColor { get; set; }
    }

    [XmlRoot(ElementName = "HumidityColor")]
    public class HumidityColor
    {
        [XmlElement(ElementName = "LowColor")]
        public LowColor LowColor { get; set; }
        [XmlElement(ElementName = "HighColor")]
        public HighColor HighColor { get; set; }
    }

    [XmlRoot(ElementName = "ProximityColor")]
    public class ProximityColor
    {
        [XmlElement(ElementName = "LowColor")]
        public LowColor LowColor { get; set; }
        [XmlElement(ElementName = "HighColor")]
        public HighColor HighColor { get; set; }
    }

    [XmlRoot(ElementName = "HourColor")]
    public class HourColor
    {
        [XmlElement(ElementName = "LowColor")]
        public LowColor LowColor { get; set; }
        [XmlElement(ElementName = "HighColor")]
        public HighColor HighColor { get; set; }
    }

    [XmlRoot(ElementName = "colorConfiguration")]
    public class ColorConfiguration
    {
        [XmlElement(ElementName = "TemperatureColor")]
        public TemperatureColor TemperatureColor { get; set; }
        [XmlElement(ElementName = "HumidityColor")]
        public HumidityColor HumidityColor { get; set; }
        [XmlElement(ElementName = "ProximityColor")]
        public ProximityColor ProximityColor { get; set; }
        [XmlElement(ElementName = "HourColor")]
        public HourColor HourColor { get; set; }
    }

}

