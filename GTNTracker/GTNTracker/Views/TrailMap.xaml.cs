using System;
using System.Collections.Generic;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{

    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class TrailMap : BasePage
    {
        private bool _fromTrailListNotification;
        private List<string> _activeTrailTrackings = new List<string>();

        public TrailMap()
        {
            InitializeComponent();

            var source = new UrlWebViewSource
            {
                Url = "http://www.grotontrails.org/Interactive_Maps.html"
            };

            if (Device.RuntimePlatform == Device.Android)
            {
                webView.Source = source;
            }
            else
            {
                webViewIOS.Source = source;
            }

            NotificationService.Instance.WebMap += HandleWebMapChange;
            NotificationService.Instance.Tracking += HandleTrackingChange;

            foreach (var trail in NotificationService.Instance.CurrentTrackingList)
            {
                _activeTrailTrackings.Add(trail);
            }

            StartTrailBtn.IsVisible = _activeTrailTrackings.Any();
            UpdateToolbar();
        }

        protected override bool OnBackButtonPressed()
        {
            UrlWebViewSource currURL = null;
            if (Device.RuntimePlatform == Device.Android)
            {
                currURL = this.webView.Source as UrlWebViewSource;
            }
            else
            {
                currURL = this.webViewIOS.Source as UrlWebViewSource;
            }

            var currURLStr = currURL.Url;

            if (currURLStr.Contains(".png") || currURLStr.Contains(".jpg"))
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    webView.GoBack();
                }
                else
                {
                    webViewIOS.GoBack();
                }
                return true;
            }

            if (_fromTrailListNotification)
            {
                NotificationService.Instance.NotifyNavigateToPage(PageManager.TrailListPageId, typeof(TrailList));
                _fromTrailListNotification = false;
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    if (webView.CanGoBack)
                    {
                        webView.GoBack();
                    }
                    else
                    {
                        //NotificationService.Instance.NotifyNavigateToPage("Groton Trail Tracker", typeof(Welcome));
                        NotificationService.Instance.NotifyNavigatePriorPage();
                    }
                }
                else
                {
                    if (webViewIOS.CanGoBack)
                    {
                        webViewIOS.GoBack();
                    }
                    else
                    {
                        //NotificationService.Instance.NotifyNavigateToPage("Groton Trail Tracker", typeof(Welcome));
                        NotificationService.Instance.NotifyNavigatePriorPage();
                    }
                }
 
            }
            return true;
        }

        private string GetMapURL(double lat, double longitude)
        {
            var rtn = string.Empty;
            var hostString = "www.grotontrails.org";
            if (!string.IsNullOrEmpty(AppStateService.Instance.TestIISHost))
            {
                hostString = AppStateService.Instance.TestIISHost + "/gtn";
                return string.Format("http://{0}/Interactive_Maps.html#map=17/{1}/{2}/GTT", hostString, lat, longitude);
            }

            // Note that we are trying to use the special waypoint handling that could be added to this page
            // so that from here we show our own GTT waypoints.
            rtn = string.Format("http://www.grotontrails.org/Interactive_Maps.html#map=16/{0}/{1}/GTT", lat, longitude);
            return rtn;
        }

        private void HandleWebMapChange(object sender, WebMapEventArgs args)
        {
            _fromTrailListNotification = true;
            var url = GetMapURL(args.Latitude, args.Longitude);
            if (!string.IsNullOrEmpty(url))
            {
                var source = new UrlWebViewSource
                {
                    Url = url
                };

                if (Device.RuntimePlatform == Device.Android)
                {
                    this.webView.Source = source;
                }
                else
                {
                    this.webViewIOS.Source = source;
                }
            }
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs args)
        {
            if (args.MonitoringState)
            {
                if (!_activeTrailTrackings.Any(t => t == args.TrailId))
                {
                    _activeTrailTrackings.Add(args.TrailId);
                }
            }
            else
            {
                _activeTrailTrackings.Remove(args.TrailId);
            }

            StartTrailBtn.IsVisible = _activeTrailTrackings.Any();
            UpdateToolbar();
        }

        private async void StartTrailBtn_Activated(object sender, EventArgs e)
        {
            var statusPage = PageManager.Instance.CurrentStatusPopup; //new CurrentStatusPage();
            var vm = ViewModelLocator.Instance.CurrentLocationVM;
            vm.UpdateCurrentPosition();
            statusPage.BindingContext = vm;
            //var trailStatus = PageManager.Instance.TrailStatus;
            //trailStatus.BindingContext = vm;
            //await Navigation.PushAsync(trailStatus);

            await Navigation.PushPopupAsync(statusPage);
            //NotificationService.Instance.NotifyNavigateToPage("Trail Tracking Status", typeof(TrailStatus));
        }
    }

}
