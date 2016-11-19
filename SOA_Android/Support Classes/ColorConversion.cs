using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Serialization;
using Android.Hardware;
using Org.Apache.Http.Conn;
using SOA_Android.Services;

namespace SOA_Android.Support_Classes
{
    public static class ColorConversion
    {
        public static ColorConfiguration DefaultColors = new ColorConfiguration
        {
            TemperatureColor = new TemperatureColor
            {
                LowColor = new LowColor
                {
                    H = "281",
                    S = "1",
                    V = "1",
                    Threshold = "0"
                },
                HighColor = new HighColor
                {
                    H = "292",
                    S = "1",
                    V = "1",
                    Threshold = "50"
                }
            },
            HumidityColor = new HumidityColor
            {
                LowColor = new LowColor
                {
                    H = "57",
                    S = "1",
                    V = "1",
                    Threshold = "0"
                },
                HighColor = new HighColor
                {
                    H = "3",
                    S = "1",
                    V = "1",
                    Threshold = "100"
                }
            },
            ProximityColor = new ProximityColor
            {
                LowColor = new LowColor
                {
                    H = "0",
                    S = "1",
                    V = "1",
                    Threshold = "0"
                },
                HighColor = new HighColor
                {
                    H = "120",
                    S = "1",
                    V = "1",
                    Threshold = "10"
                }
            },
            HourColor = new HourColor
            {
                LowColor = new LowColor
                {
                    H = "288",
                    S = "1",
                    V = "1",
                    Threshold = "0"
                },
                HighColor = new HighColor
                {
                    H = "17",
                    S = "1",
                    V = "1",
                    Threshold = "86400"
                }
            }
        };
        
        public static Hsv ToHsv(this Color color)
        {
            var hsv = new Hsv();

            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            hsv.H = Color.FromArgb(255, color.R, color.G, color.B).GetHue();
            hsv.S = (max <= 0) ? 0 : 1d - (1d * min / max);
            hsv.V = max / 255d;

            return hsv;
        }

        public static Color ToColor(this Hsv item)
        {
            var range = Convert.ToInt32(Math.Floor(item.H / 60.0)) % 6;
            var f = item.H / 60.0 - Math.Floor(item.H / 60.0);

            var v = Convert.ToInt32(item.V * 255.0);
            var p = Convert.ToInt32(v * (1 - item.S));
            var q = Convert.ToInt32(v * (1 - f * item.S));
            var t = Convert.ToInt32(v * (1 - (1 - f) * item.S));

            switch (range)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
            }
            return Color.FromArgb(255, v, p, q);
        }

        public static ColorConfiguration GetColorSetupFromXML()
        {
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            var filePath = Path.Combine(documentsPath, "SOA-ICI/ColorConfiguration.xml");
            var xs = new XmlSerializer(typeof(ColorConfiguration));
            if (!File.Exists(filePath))
                BuildDefaultColorConfiguration();
            using (var sr = new StreamReader(filePath))
            {
                try
                {
                    return (ColorConfiguration) xs.Deserialize(sr);
                }
                catch
                {
                    BuildDefaultColorConfiguration();
                    return (ColorConfiguration)xs.Deserialize(sr);
                }
            }
        }


        public static string GetXMLString()
        {
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            var filePath = Path.Combine(documentsPath, "SOA-ICI/ColorConfiguration.xml");
            var xs = new XmlSerializer(typeof(ColorConfiguration));
            if (!File.Exists(filePath))
                BuildDefaultColorConfiguration();
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static void BuildDefaultColorConfiguration()
        {
            float maxRange = 100;
            if(ProximitySensorService.ProximitySensor != null)
                maxRange = (ProximitySensorService.ProximitySensor.MaximumRange < 10) ? ProximitySensorService.ProximitySensor.MaximumRange : 10;
            DefaultColors.ProximityColor.HighColor.Threshold = maxRange.ToString();
            DefaultColors.SaveToXML();
        }

        public static void SaveToXML(this ColorConfiguration colors)
        {
            CreateXMLDirectoryIfDoesNotExist();
            var xs = new XmlSerializer(typeof(ColorConfiguration));
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            var filePath = Path.Combine(documentsPath, "SOA-ICI/ColorConfiguration.xml");
            using (var tw = new StreamWriter(filePath))
            {
                xs.Serialize(tw, colors);
            }
        }

        public static void CreateXMLDirectoryIfDoesNotExist()
        {
            var dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/SOA-ICI/");
            if (!dir.Exists())
                dir.Mkdirs();
        }

        public static Android.Graphics.Color ToAndroidColor(this Color color)
        {
            return new Android.Graphics.Color
            {
                R = color.R,
                G = color.G,
                B = color.B,
                A = 255
            };
        }

        public static Color ToWindowsColor(this Android.Graphics.Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }

        public static Hsv ToHsv(this LowColor color)
        {
            return new Hsv
            {
                H = double.Parse(color.H),
                S = double.Parse(color.S),
                V = double.Parse(color.V),
            };
        }

        public static Hsv ToHsv(this HighColor color)
        {
            return new Hsv
            {
                H = double.Parse(color.H),
                S = double.Parse(color.S),
                V = double.Parse(color.V),
            };
        }

        public static Color GetGradientColor(double currentValue, LowColor lowerColor, HighColor higherColor, double defaultThreshold)
        {
            var lowColor = lowerColor.ToHsv();
            var highColor = higherColor.ToHsv();
            var value = currentValue;
            if (currentValue < double.Parse(lowerColor.Threshold))
                value = double.Parse(lowerColor.Threshold);
            else if (currentValue > double.Parse(higherColor.Threshold))
                value = double.Parse(higherColor.Threshold);
            value = value * 100 / defaultThreshold / 100;
            var gradient = new Hsv
            {
                H = lowColor.H + (value * (highColor.H - lowColor.H)),
                S = lowColor.S + (value * (highColor.S - lowColor.S)),
                V = lowColor.V + (value * (highColor.V - lowColor.V))
            };
            var gradientColor = gradient.ToColor();
            return gradientColor;
        }
    }
}