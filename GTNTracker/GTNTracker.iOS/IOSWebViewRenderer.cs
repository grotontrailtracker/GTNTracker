using System;
using System.Diagnostics;
using System.Linq;
using Foundation;
using GTNTracker.iOS;
using GTNTracker.Views;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(IOSWebView), typeof(CustomWebViewTerribleBugRenderer))]
namespace GTNTracker.iOS
{
    public class CustomWebViewTerribleBugRenderer : Xamarin.Forms.Platform.iOS.WebViewRenderer
    {
        public override void LoadRequest(NSUrlRequest r)
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();

            // check if this was(n't) called from LoadUrl method, since we can't override it...
            if (!stackFrames.Any(x => x.GetMethod().Name == "LoadUrl" && x.GetMethod().DeclaringType == typeof(Xamarin.Forms.Platform.iOS.WebViewRenderer)))
            {
                // called from other place so just call base and return
                base.LoadRequest(r);
                return;
            }

            // reconstruct original URL
            var decodedStringUrl = new NSString(r.Url.AbsoluteString).CreateStringByReplacingPercentEscapes(NSStringEncoding.UTF8);

            // call load request with original string
            base.LoadRequest(new NSUrlRequest(new NSUrl(decodedStringUrl)));
        }
    }
}
