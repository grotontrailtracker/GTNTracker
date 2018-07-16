using Android.Content;
using Android.Webkit;
using GTNTracker;
using GTNTracker.Droid;
using GTNTracker.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GeoWebView), typeof(GeoWebViewRenderer))]
namespace GTNTracker.Droid
{
    public class GeoWebViewRenderer : WebViewRenderer
    {
        public GeoWebViewRenderer() : base(Android.App.Application.Context)
        {
        }

        public GeoWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            Control.Settings.JavaScriptEnabled = true;
            Control.SetWebChromeClient(new MyWebClient());
        }
    }

    public class MyWebClient : WebChromeClient
    {
        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            callback.Invoke(origin, true, false);
        }
    }
}