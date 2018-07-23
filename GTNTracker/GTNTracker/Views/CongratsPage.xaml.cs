using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CongratsPage : PopupPage
	{
		public CongratsPage ()
		{
			InitializeComponent ();
            BindingContext = new CongratsViewModel();
		}

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await Navigation.PopAllPopupAsync();
        }

        private async void OnGotoTrailsButtonTapped(object sender, EventArgs e)
        {
            await Navigation.PopAllPopupAsync();
            Device.OpenUri(new Uri("http://www.grotontrails.org"));
        }

        private async void SendGTNEmail(object sender, EventArgs e)
        {
            await Navigation.PopAllPopupAsync();

            string body = "Groton Trail Tracker - Completed All Trails\n";
            body += $"version: {AppStateService.Instance.AppVersion}\n";

            // put the person's name into the message body
            if (!string.IsNullOrEmpty(NameEntry.Text))
            {
                body += $"Name: {NameEntry.Text}\n";
            }

            // grab all the trail defs
            foreach (var trailDef in TrailDefService.Instance.TrailDefinitions)
            {
                var completedList = TrailVisitService.Instance.GetVisits(trailDef.Identifier);
                var completed = DateTime.Now;
                if (completedList.Any())
                {
                    completed = completedList.Max(t => t.Completed);
                }

                var msg = $"Trail: {trailDef.Name}, Completed: {completed.ToLocalTime()}";
                body += msg + "\n";

                // loop through the individual visits
                if (completedList.Any())
                {
                    foreach (var visit in completedList)
                    {
                        var def = TrailDefService.Instance.GetRegionDefinition(trailDef.Identifier).FirstOrDefault(d => d.Identifier == visit.RegionIdentifier);
                        string name = def != null ? def.Name : "<None>";
                        msg = $"\t - {name}, Visited: {visit.Completed.ToLocalTime()}";
                        body += msg + "\n";
                    }
                }
            }

            List<string> recipients = new List<string>();
            List<string> ccs = new List<string>();
            string subject = string.Empty;
            
            string bodyHtml = string.Empty;
            recipients.Add(AppStateService.Instance.EmailAddress);
            subject = "GTT Completed Trails";

            DependencyService.Get<IEmailService>().CreateEmail(recipients, ccs, subject, body, bodyHtml);
        }
    }

    
}