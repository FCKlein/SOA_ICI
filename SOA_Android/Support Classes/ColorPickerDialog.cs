using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SOA_Android.Activities;
using Stream = System.IO.Stream;
using SOA_Android.Support_Classes;

namespace SOA_Android.Support_Classes
{
    public enum ColorValue
    {
        LowColor,
        HighColor
    }

    public class ColorPickerDialog
    {
        #region Fields
        private AlertDialog.Builder dialogBuilder;
        private SeekBar SliderRed, SliderGreen, SliderBlue;
        private Button ColorPreview;
        private ColorConfiguration ColorConfiguration;
        private Type ColorToChange;
        private ColorValue ValueToChange;
        private ConfigActivity Activity;
        public Android.Graphics.Color SetColor { get; private set; }
        #endregion

        #region Constructors
        public ColorPickerDialog(Context context, View dialogView, Android.Graphics.Color currentColor, Type colorToChange, ColorValue valueToChange)
        {
            dialogBuilder = new AlertDialog.Builder(context);
            dialogBuilder.SetView(dialogView);

            Activity = (ConfigActivity) context;

            ColorConfiguration = ColorConversion.GetColorSetupFromXML();
            ColorToChange = colorToChange;
            ValueToChange = valueToChange;

            SliderRed = (SeekBar) dialogView.FindViewById(Resource.Id.sliderRed);
            SliderGreen = (SeekBar) dialogView.FindViewById(Resource.Id.sliderGreen);
            SliderBlue = (SeekBar) dialogView.FindViewById(Resource.Id.sliderBlue);
            
            SliderRed.Progress = currentColor.R;
            SliderGreen.Progress = currentColor.G;
            SliderBlue.Progress = currentColor.B;
            currentColor.A = 255;

            SetColor = currentColor;

            ColorPreview = (Button) dialogView.FindViewById(Resource.Id.btnColor);

            ColorPreview.SetBackgroundColor(currentColor);

            SliderRed.ProgressChanged += Slider_ProgressChanged;

            SliderGreen.ProgressChanged += Slider_ProgressChanged;

            SliderBlue.ProgressChanged += Slider_ProgressChanged;

            dialogBuilder.SetPositiveButton("Aceptar", btnAceptar_Click);

            dialogBuilder.SetNeutralButton("Cancelar", (sender, args) =>
            {
                SetColor = new Android.Graphics.Color(currentColor.R, currentColor.G, currentColor.B);
                dialogBuilder.Dispose();
            });

            dialogBuilder.Show();
        }

        protected void Slider_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!e.FromUser) return;
            SetColor = new Android.Graphics.Color(SliderRed.Progress, SliderGreen.Progress, SliderBlue.Progress);
            ColorPreview.SetBackgroundColor(SetColor);
        }

        protected void btnAceptar_Click(object sender, DialogClickEventArgs e)
        {
            var newColor = Color.FromArgb(SliderRed.Progress, SliderGreen.Progress, SliderBlue.Progress).ToHsv();
            var color = Color.FromArgb(SliderRed.Progress, SliderGreen.Progress, SliderBlue.Progress).ToAndroidColor();
            LowColor newLow = null;
            HighColor newHigh = null;

            switch (ValueToChange)
            {
                case ColorValue.LowColor:
                    newLow = new LowColor
                    {
                        H = newColor.H.ToString(),
                        S = newColor.S.ToString(),
                        V = newColor.V.ToString(),
                    };
                    break;
                case ColorValue.HighColor:
                    newHigh = new HighColor
                    {
                        H = newColor.H.ToString(),
                        S = newColor.S.ToString(),
                        V = newColor.V.ToString(),
                    };
                    break;
            }

            if (ColorToChange == typeof (TemperatureColor))
            {
                ColorConfiguration.TemperatureColor.LowColor = newLow ?? ColorConfiguration.TemperatureColor.LowColor;
                ColorConfiguration.TemperatureColor.HighColor = newHigh ?? ColorConfiguration.TemperatureColor.HighColor;
                if(newLow != null)
                    ((Button) Activity.FindViewById(Resource.Id.btnLowTemp)).SetBackgroundColor(color);
                else
                    ((Button) Activity.FindViewById(Resource.Id.btnHighTemp)).SetBackgroundColor(color);
            }
            else if (ColorToChange == typeof(HumidityColor))
            {
                ColorConfiguration.HumidityColor.LowColor = newLow ?? ColorConfiguration.HumidityColor.LowColor;
                ColorConfiguration.HumidityColor.HighColor = newHigh ?? ColorConfiguration.HumidityColor.HighColor;
                if (newLow != null)
                    ((Button)Activity.FindViewById(Resource.Id.btnLowHumidity)).SetBackgroundColor(color);
                else
                    ((Button)Activity.FindViewById(Resource.Id.btnHighHumidity)).SetBackgroundColor(color);
            }
            else if (ColorToChange == typeof(ProximityColor))
            {
                ColorConfiguration.ProximityColor.LowColor = newLow ?? ColorConfiguration.ProximityColor.LowColor;
                ColorConfiguration.ProximityColor.HighColor = newHigh ?? ColorConfiguration.ProximityColor.HighColor;
                if (newLow != null)
                    ((Button)Activity.FindViewById(Resource.Id.btnLowProximity)).SetBackgroundColor(color);
                else
                    ((Button)Activity.FindViewById(Resource.Id.btnHighProximity)).SetBackgroundColor(color);
            }
            else if (ColorToChange == typeof(HourColor))
            {
                ColorConfiguration.HourColor.LowColor = newLow ?? ColorConfiguration.HourColor.LowColor;
                ColorConfiguration.HourColor.HighColor = newHigh ?? ColorConfiguration.HourColor.HighColor;
                if (newLow != null)
                    ((Button)Activity.FindViewById(Resource.Id.btnLowHour)).SetBackgroundColor(color);
                else
                    ((Button)Activity.FindViewById(Resource.Id.btnHighHour)).SetBackgroundColor(color);
            }

            ColorConfiguration.SaveToXML();
        }

        #endregion
    }
}