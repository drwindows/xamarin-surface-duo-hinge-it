
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Device.Display;
using System;
using Xamarin.Forms;

namespace HingeIt.Droid
{
    [Activity(
        Label = "HingeIt",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize
    )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        #region Private members 

        HingeSensor hingeSensor;
        int lastHingeAngle = 0;

        #endregion

        #region Life cycle

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            Xamarin.Forms.DualScreen.DualScreenService.Init(this);
            hingeSensor = new HingeSensor(this);

            LoadApplication(new App());
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (hingeSensor?.HasHinge ?? false)
            {
                hingeSensor.OnSensorChanged += HingeSensor_OnSensorChanged;
                hingeSensor.StartListening();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (hingeSensor?.HasHinge ?? false)
            {
                hingeSensor.StopListening();
                hingeSensor.OnSensorChanged -= HingeSensor_OnSensorChanged;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion

        #region Event handler

        private void HingeSensor_OnSensorChanged(object sender, HingeSensor.HingeSensorChangedEventArgs e)
        {
            if (e.HingeAngle == lastHingeAngle) return;
            lastHingeAngle = e.HingeAngle;
            Console.WriteLine($"Hinge Sensor Changed: {e.HingeAngle}");
            MessagingCenter.Send($"{e.HingeAngle}", "HingeSensorChanged");
        }

        #endregion
    }
}