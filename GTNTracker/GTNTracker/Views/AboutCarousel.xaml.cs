using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GTNTracker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutCarousel : CarouselPage
	{
        public bool IsAndroid { get; set; }

		public AboutCarousel ()
		{
            IsAndroid = Device.RuntimePlatform == Device.Android;
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            BindingContext = this;

			InitializeComponent ();

            AboutText.Text = "This mobile app will help you explore some of the beautiful trails in the Groton Trails Network. "
                            + "Each of these trails has been mapped with a set of GPS waypoints for you to visit as you explore the trail. "
                            + "When you reach the waypoint GPS coordinates, the app will record the visit and show your trail progress. ";

            Instruction1.Text = "Once you've arrived at the trail, select the Trail List page and find the button for your trail. "
                            + "Tap the button and it will display all the waypoints you need to visit to complete the trail. "
                            + "You can tap each waypoint which will display some information about it. ";

            var s = new FormattedString();
            s.Spans.Add(new Span { Text = "To start tracking the trail, tap the " });
            s.Spans.Add(new Span { Text = " Start Trail Tracking ", ForegroundColor = Color.FromHex("FF616161"), BackgroundColor = Color.FromHex("FFBDBDBD"), FontSize = 16 });
            s.Spans.Add(new Span { Text = " button. Now you're ready to go hiking and explore the trail. You'll get a notification when you reach a waypoint. " });
            s.Spans.Add(new Span { Text = "When you're done visiting the trail, tap the " });
            s.Spans.Add(new Span { Text = " Stop Trail Tracking ", ForegroundColor = Color.FromHex("FF616161"), BackgroundColor = Color.FromHex("FFBDBDBD"), FontSize = 16 });
            s.Spans.Add(new Span { Text = " button. " });
            //Instruction2.Text = "To start tracking the trail, tap the 'Start Trail Tracking' button. Now you're ready to go hiking and explore the trail. "
            //                + "You'll get a notification when you reach a waypoint. "
            //                + "When you're done visiting the trail, tap the 'Stop Trail Tracking' button. ";
            Instruction2.FormattedText = s;

            Instruction3.Text = "When you've visited all the waypoints, the app will track that you've completed the trail. "
                            + "And, when you've completed all the trails, the app will let you email the GTN organization of your successful completion!";
            PicInstructions.Text = "You can zoom the pictures by clicking on a thumbnail or double click a picture. Just click once to close.";

            ThankYouText.Text = "I would like to thank the Groton Trails Committee for sponsoring and helping me with this project. "
                            + "Also, I'd like to thank the scouts, leaders and parents of Troop 1 West Groton for their help in developing this app and mapping the trails.";
        }

        protected override bool OnBackButtonPressed()
        {
            NotificationService.Instance.NotifyNavigatePriorPage();
            return true;
        }
    }
}