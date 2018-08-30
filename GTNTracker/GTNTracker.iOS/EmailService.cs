using System;
using System.Collections.Generic;
using GTNTracker.Interfaces;
using GTNTracker.iOS;
using MessageUI;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(EmailService))]
namespace GTNTracker.iOS
{ 
    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public void CreateEmail(List<string> emailAddresses, List<string> ccs, string subject, string body, string htmlBody)
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                var mailController = new MFMailComposeViewController();
                mailController.SetToRecipients(emailAddresses.ToArray());
                mailController.SetSubject(subject);
                mailController.SetMessageBody(body, false);
                GetController().PresentViewController(mailController, true, null);
                mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                {
                    Console.WriteLine(args.Result.ToString());
                    args.Controller.DismissViewController(true, null);
                };
            }
            else
            {
                Console.WriteLine("===> Unable to send email!!!!!");
            }
        }

        public void EmailWaypoint(string waypointData, string filePath, int id)
        {

            if (MFMailComposeViewController.CanSendMail)
            {
                List<string> emailAddresses = new List<string>();
                emailAddresses.Add("grotontrailtracker@gmail.com");

                UIImage image = UIImage.FromFile(filePath);
                var mailController = new MFMailComposeViewController();
                mailController.SetToRecipients(emailAddresses.ToArray());
                mailController.SetSubject("waypoint");
                mailController.SetMessageBody(waypointData, false);
                mailController.AddAttachmentData(image.AsJPEG(), "image/JPG", "waypoint.jpg");
                //GetController().PresentViewController(mailController, true, null);
                mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                {
                    Console.WriteLine("---> Mail Result:" + args.Result.ToString() + "for id:" + id);
                    MessagingCenter.Send<WaypointEmailed, WaypointEMailedArgs>(new WaypointEmailed(), WaypointEmailed.MessageString, new WaypointEMailedArgs(id));
                    args.Controller.DismissViewController(true, null);
                };
                GetController().PresentViewController(mailController, true, null);
            }
            else
            {
                Console.WriteLine("===> Unable to send email!!!!!");
            }
        }

        private static UIViewController GetController()
        {
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }
    }
}
