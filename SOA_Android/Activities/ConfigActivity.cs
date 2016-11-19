using System;
using System.Collections.Specialized;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using SOA_Android.Support_Classes;

namespace SOA_Android.Activities
{
    [Activity]
    public class ConfigActivity : Activity
    {
        private Button btnLowTemp, btnHighTemp, btnLowHumidity, btnHighHumidity, btnLowProximity, btnHighProximity, btnLowHour, btnHighHour;
        private EditText txtLowTemp, txtHighTemp, txtLowHumidity, txtHighHumidity;

        private ColorConfiguration colors;

        private InputMethodManager imm;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Config);

            colors = ColorConversion.GetColorSetupFromXML();
            var color = new Hsv
            {
                H = double.Parse(colors.TemperatureColor.LowColor.H),
                S = double.Parse(colors.TemperatureColor.LowColor.S),
                V = double.Parse(colors.TemperatureColor.LowColor.V)
            }.ToColor().ToAndroidColor();
            btnLowTemp = (Button) FindViewById(Resource.Id.btnLowTemp);
            btnLowTemp.Click += btnLowTemp_Click;
            btnLowTemp.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.TemperatureColor.HighColor.H),
                S = double.Parse(colors.TemperatureColor.HighColor.S),
                V = double.Parse(colors.TemperatureColor.HighColor.V)
            }.ToColor().ToAndroidColor();

            btnHighTemp = (Button)FindViewById(Resource.Id.btnHighTemp);
            btnHighTemp.Click += btnHighTemp_Click;
            btnHighTemp.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.HumidityColor.LowColor.H),
                S = double.Parse(colors.HumidityColor.LowColor.S),
                V = double.Parse(colors.HumidityColor.LowColor.V)
            }.ToColor().ToAndroidColor();

            btnLowHumidity = (Button)FindViewById(Resource.Id.btnLowHumidity);
            btnLowHumidity.Click += btnLowHumidity_Click;
            btnLowHumidity.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.HumidityColor.HighColor.H),
                S = double.Parse(colors.HumidityColor.HighColor.S),
                V = double.Parse(colors.HumidityColor.HighColor.V)
            }.ToColor().ToAndroidColor();

            btnHighHumidity = (Button)FindViewById(Resource.Id.btnHighHumidity);
            btnHighHumidity.Click += btnHighHumidity_Click;
            btnHighHumidity.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.ProximityColor.LowColor.H),
                S = double.Parse(colors.ProximityColor.LowColor.S),
                V = double.Parse(colors.ProximityColor.LowColor.V)
            }.ToColor().ToAndroidColor();

            btnLowProximity = (Button)FindViewById(Resource.Id.btnLowProximity);
            btnLowProximity.Click += btnLowProximity_Click;
            btnLowProximity.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.ProximityColor.HighColor.H),
                S = double.Parse(colors.ProximityColor.HighColor.S),
                V = double.Parse(colors.ProximityColor.HighColor.V)
            }.ToColor().ToAndroidColor();

            btnHighProximity = (Button)FindViewById(Resource.Id.btnHighProximity);
            btnHighProximity.Click += btnHighProximity_Click;
            btnHighProximity.SetBackgroundColor(color);
            
            color = new Hsv
            {
                H = double.Parse(colors.HourColor.LowColor.H),
                S = double.Parse(colors.HourColor.LowColor.S),
                V = double.Parse(colors.HourColor.LowColor.V)
            }.ToColor().ToAndroidColor();

            btnLowHour = (Button)FindViewById(Resource.Id.btnLowHour);
            btnLowHour.Click += btnLowHour_Click;
            btnLowHour.SetBackgroundColor(color);

            color = new Hsv
            {
                H = double.Parse(colors.HourColor.HighColor.H),
                S = double.Parse(colors.HourColor.HighColor.S),
                V = double.Parse(colors.HourColor.HighColor.V)
            }.ToColor().ToAndroidColor();

            btnHighHour = (Button)FindViewById(Resource.Id.btnHighHour);
            btnHighHour.Click += btnHighHour_Click;
            btnHighHour.SetBackgroundColor(color);

            txtLowTemp = (EditText) FindViewById(Resource.Id.txtLowTempValue);
            txtHighTemp = (EditText)FindViewById(Resource.Id.txtHighTempValue);
            txtLowHumidity = (EditText)FindViewById(Resource.Id.txtLowHumidityValue);
            txtHighHumidity = (EditText)FindViewById(Resource.Id.txtHighHumidityValue);

            txtLowTemp.Text = colors.TemperatureColor.LowColor.Threshold;
            txtHighTemp.Text = colors.TemperatureColor.HighColor.Threshold;
            txtLowHumidity.Text = colors.HumidityColor.LowColor.Threshold;
            txtHighHumidity.Text = colors.HumidityColor.HighColor.Threshold;

            txtLowTemp.FocusChange += UpdateValues;
            txtHighTemp.FocusChange += UpdateValues;
            txtLowHumidity.FocusChange += UpdateValues;
            txtHighHumidity.FocusChange += UpdateValues;
            
            imm = (InputMethodManager)GetSystemService(InputMethodService);

            var layout = (LinearLayout) FindViewById(Resource.Id.parentLayout);
            layout.Click += ConfigActivity_Click;
            layout.RequestFocus();
        }

        private void ConfigActivity_Click(object sender, System.EventArgs e)
        {
            imm.HideSoftInputFromWindow(txtLowTemp.WindowToken, 0);
            imm.HideSoftInputFromWindow(txtHighTemp.WindowToken, 0);
            imm.HideSoftInputFromWindow(txtLowHumidity.WindowToken, 0);
            imm.HideSoftInputFromWindow(txtHighHumidity.WindowToken, 0);
        }

        public override void OnBackPressed()
        {
            colors = ColorConversion.GetColorSetupFromXML();
            colors.TemperatureColor.LowColor.Threshold = txtLowTemp.Text;
            colors.TemperatureColor.HighColor.Threshold = txtHighTemp.Text;
            colors.HumidityColor.LowColor.Threshold = txtLowHumidity.Text;
            colors.HumidityColor.HighColor.Threshold = txtHighHumidity.Text;
            colors.SaveToXML();
            var dweetResponse = Http.Post(Constants.DweetPostColorsHttp, new NameValueCollection
            {
                { "type", "colors" },
                { "value", ColorConversion.GetXMLString() }
            });
            base.OnBackPressed();
            var layout = (LinearLayout)FindViewById(Resource.Id.parentLayout);
            layout.RequestFocus();
        }

        protected void btnLowTemp_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginLowColorPickerDialog(colors.TemperatureColor.LowColor,
                typeof(TemperatureColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.TemperatureColor.LowColor.H = hsv.H.ToString();
            colors.TemperatureColor.LowColor.S = hsv.S.ToString();
            colors.TemperatureColor.LowColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnLowTemp.SetBackgroundColor(newColor);
        }

        protected void btnHighTemp_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginHighColorPickerDialog(colors.TemperatureColor.HighColor,
                typeof(TemperatureColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.TemperatureColor.HighColor.H = hsv.H.ToString();
            colors.TemperatureColor.HighColor.S = hsv.S.ToString();
            colors.TemperatureColor.HighColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnHighTemp.SetBackgroundColor(newColor);
        }

        protected void btnLowHumidity_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginLowColorPickerDialog(colors.HumidityColor.LowColor,
                typeof(HumidityColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.HumidityColor.LowColor.H = hsv.H.ToString();
            colors.HumidityColor.LowColor.S = hsv.S.ToString();
            colors.HumidityColor.LowColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnLowHumidity.SetBackgroundColor(newColor);
        }

        protected void btnHighHumidity_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginHighColorPickerDialog(colors.HumidityColor.HighColor,
                typeof(HumidityColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.HumidityColor.HighColor.H = hsv.H.ToString();
            colors.HumidityColor.HighColor.S = hsv.S.ToString();
            colors.HumidityColor.HighColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnHighHumidity.SetBackgroundColor(newColor);
        }

        protected void btnLowProximity_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginLowColorPickerDialog(colors.ProximityColor.LowColor,
                typeof(ProximityColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.ProximityColor.LowColor.H = hsv.H.ToString();
            colors.ProximityColor.LowColor.S = hsv.S.ToString();
            colors.ProximityColor.LowColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnLowProximity.SetBackgroundColor(newColor);
        }

        protected void btnHighProximity_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginHighColorPickerDialog(colors.ProximityColor.HighColor,
                typeof(ProximityColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.ProximityColor.HighColor.H = hsv.H.ToString();
            colors.ProximityColor.HighColor.S = hsv.S.ToString();
            colors.ProximityColor.HighColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnHighProximity.SetBackgroundColor(newColor);
        }


        protected void btnLowHour_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginLowColorPickerDialog(colors.HourColor.LowColor,
                typeof(HourColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.HourColor.LowColor.H = hsv.H.ToString();
            colors.HourColor.LowColor.S = hsv.S.ToString();
            colors.HourColor.LowColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnLowHour.SetBackgroundColor(newColor);
        }

        protected void btnHighHour_Click(object sender, EventArgs e)
        {
            colors = ColorConversion.GetColorSetupFromXML();
            var newColor = BeginHighColorPickerDialog(colors.HourColor.HighColor,
                typeof(HourColor));
            var hsv = newColor.ToWindowsColor().ToHsv();
            colors.HourColor.HighColor.H = hsv.H.ToString();
            colors.HourColor.HighColor.S = hsv.S.ToString();
            colors.HourColor.HighColor.V = hsv.V.ToString();
            colors.SaveToXML();
            btnHighHour.SetBackgroundColor(newColor);
        }

        private Color BeginLowColorPickerDialog(LowColor color, Type ColorType)
        {
            var hsv = new Hsv
            {
                H = double.Parse(color.H),
                S = double.Parse(color.S),
                V = double.Parse(color.V)
            };
            var colorPickerDialog = new ColorPickerDialog(this, LayoutInflater.Inflate(Resource.Layout.ColorPickerDialog, null), hsv.ToColor().ToAndroidColor(), ColorType, ColorValue.LowColor);
            return colorPickerDialog.SetColor;
        }

        private Color BeginHighColorPickerDialog(HighColor color, Type ColorType)
        {
            var hsv = new Hsv
            {
                H = double.Parse(color.H),
                S = double.Parse(color.S),
                V = double.Parse(color.V)
            };
            var colorPickerDialog = new ColorPickerDialog(this, LayoutInflater.Inflate(Resource.Layout.ColorPickerDialog, null), hsv.ToColor().ToAndroidColor(), ColorType, ColorValue.HighColor);
            return colorPickerDialog.SetColor;
        }

        private void UpdateValues(object sender, EventArgs e)
        {
            if (((EditText) sender).IsFocused) return;
            colors = ColorConversion.GetColorSetupFromXML();
            colors.TemperatureColor.LowColor.Threshold = txtLowTemp.Text;
            colors.TemperatureColor.HighColor.Threshold = txtHighTemp.Text;
            colors.HumidityColor.LowColor.Threshold = txtLowHumidity.Text;
            colors.HumidityColor.HighColor.Threshold = txtHighHumidity.Text;
            colors.SaveToXML();
        }
    }
}