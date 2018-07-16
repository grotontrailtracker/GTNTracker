using System;
using UserNotifications;

namespace GTNTracker.iOS
{

    public class NotificationDelegate : UNUserNotificationCenterDelegate
    {
        #region Constructors
        public NotificationDelegate()
        {
        }
        #endregion

        #region Override Methods
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification
            Console.WriteLine("Active Notification: {0}", notification);

            UNNotificationRequest request = notification.Request;
            var idStr = request.Identifier;
            var content = request.Content;

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
        #endregion
    }
}
