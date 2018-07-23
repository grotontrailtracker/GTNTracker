using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Compat;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using GTNTracker.Droid;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(EmailService))]
namespace GTNTracker.Droid
{
    public class EmailService : IEmailService
    {
        public void CreateEmail(List<string> emailAddresses, List<string> ccs, string subject, string body, string htmlBody)
        {
            var email = new Intent(Android.Content.Intent.ActionSend);

            if (emailAddresses?.Count > 0)
            {
                email.PutExtra(Android.Content.Intent.ExtraEmail, emailAddresses.ToArray());
            }

            if (ccs?.Count > 0)
            {
                email.PutExtra(Android.Content.Intent.ExtraCc, ccs.ToArray());
            }

            email.PutExtra(Android.Content.Intent.ExtraSubject, subject);

            email.PutExtra(Android.Content.Intent.ExtraText, body);

            email.PutExtra(Android.Content.Intent.ExtraHtmlText, htmlBody);

            email.SetType("message/rfc822");

            MainActivity.SharedInstance.StartActivity(email);

        }
        
        public void EmailWaypoint(string waypointData, string filePath, int id)
        {
            var email = new Intent(Android.Content.Intent.ActionSend);
            List<string> address = new List<string>();
            address.Add(AppStateService.Instance.EmailAddress);
            email.PutExtra(Android.Content.Intent.ExtraEmail, address.ToArray());
            email.PutExtra(Android.Content.Intent.ExtraSubject, "Waypoint");
            email.PutExtra(Android.Content.Intent.ExtraText, waypointData);
            var file = new Java.IO.File(filePath);
            var uri = FileProvider.GetUriForFile(Android.App.Application.Context, "org.grotontrails.GrotonTrailTracker.fileprovider", file);

            email.PutExtra(Intent.ExtraStream, uri);
            email.SetType("message/rfc822");

            MainActivity.SharedInstance.WaypointMailedId = id;

            //MainActivity.SharedInstance.StartActivity(email);
            MainActivity.SharedInstance.StartActivityForResult(email, 12345);
        }
    }
}