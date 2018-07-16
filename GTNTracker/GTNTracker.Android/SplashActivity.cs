using System;
using System.Threading;
using Android.App;
using Android.OS;

namespace GTNTracker.Droid
{
    [Activity(Label = "GTN Tracker", Icon = "@drawable/ic_gtnRound", Theme = "@style/MyTheme.Splash", MainLauncher = false, NoHistory = true)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof (SplashActivity).Name;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                SetContentView(Resource.Layout.dumbLayout);

                ThreadPool.QueueUserWorkItem(o => LoadActivity());
                //StartActivity(typeof(MainActivity));

                //StartActivity(typeof(MainActivity));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"====>Exception in Create Splash Activity: {ex.Message}");
            }
        }

        private void LoadActivity()
        {
            RunOnUiThread(() => StartActivity(typeof(MainActivity)));
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }
    }
}