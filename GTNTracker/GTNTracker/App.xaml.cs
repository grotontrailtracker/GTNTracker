using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using GTNTracker.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace GTNTracker
{
    public partial class App : Application
    {
        private AppStateService _appState;

        private bool _serviceLoaded;
        private bool _waitingForNotificationDestroy;

        public App()
        {
            InitializeComponent();

            _appState = AppStateService.Instance;

            MainPage = new AppRootPage();

            MessagingCenter.Subscribe<NotificationDestroyEvent>(this, NotificationDestroyEvent.MessageString, 
                                    (sender) => { HandleNotificationDestroyEvent(); });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Debug.WriteLine("***===> App has started");
            _appState.IsAppAwake = true;

            if (Application.Current.Properties.ContainsKey("ActiveTrail"))
            {
                var activeTrailId = Application.Current.Properties["ActiveTrail"] as string;
                if (!string.IsNullOrEmpty(activeTrailId))
                {
                    AppStateService.Instance.ActiveTrailId = activeTrailId;
                    // and reset the trail since we're acting upon it
                    Application.Current.Properties.Remove("ActiveTrail");
                    Application.Current.SavePropertiesAsync();
                }
            }

            if (AppStateService.Instance.IsNotificationLaunch)
            {
                var monitoredTrail = GeoFenceService.GetCurrentMonitoredTrail();
                if (!string.IsNullOrEmpty(monitoredTrail))
                {
                    // when using the Notification intent, we must wait until Android destroys the original main activity
                    // which will shutdown the service in OnDestroy, then when it sends the event message, we can safely
                    // restart the GPS service
                    _waitingForNotificationDestroy = true;
                }

                // make sure it's not from a notification left after the app was destroyed 
                // Check the TrailDefService to see if it's loaded.
                // TODO: Sometimes this causes an exception if we return here and something downstream happens with an unhandled exception!
                //if (TrailDefService.Instance.IsStarted)
                //{
                //    return;
                //}
            }

            // Make sure we didn't leave the service running from the last time we were here, if it is force it
            // to shutdown
            if (GeoFenceService.IsRunning())
            {
                MessagingCenter.Send(new StopGeofencing(), StopGeofencing.MessageString, new StopGeofencingArgs(true));
            }

            InitializeServices();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Debug.WriteLine(string.Format("***====> App has gone to sleep, active: {0}", 
                !string.IsNullOrEmpty(AppStateService.Instance.ActiveTrailId) ? AppStateService.Instance.ActiveTrailId : "<none>"));
            _appState.IsAppAwake = false;

            if (!string.IsNullOrEmpty(AppStateService.Instance.ActiveTrailId))
            {
                Application.Current.Properties["ActiveTrail"] = AppStateService.Instance.ActiveTrailId;
            }
            else
            {
                Application.Current.Properties.Remove("ActiveTrail");
                Application.Current.SavePropertiesAsync();
            }

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Debug.WriteLine("***===> App has resumed");
            _appState.IsAppAwake = true;
            if (Application.Current.Properties.ContainsKey(""))
            {
                Application.Current.Properties.Remove("ActiveTrail");
                Application.Current.SavePropertiesAsync();
            }
        }

        private void HandleNotificationDestroyEvent()
        {
            if (_waitingForNotificationDestroy)
            {
                _waitingForNotificationDestroy = false;
                MessagingCenter.Send(new StartGeofencing(), StartGeofencing.MessageString);
            }
        }

        // stuff to put in the bootstrapper class
        public void InitializeServices()
        {

            if (!_serviceLoaded)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DoPermissionCheckAsync();
                    if (!TrailDefService.Instance.IsStarted)
                    {
                        TrailDefService.Instance.TrailDefsUpdated += Handle_TrailDefsUpdated;
                        TrailDefService.Instance.Start();
                    }
                    else
                    {
                        // reinit from notification.
                        FinishInitializations();
                    }
                });
            }
        }

        private async Task DoPermissionCheckAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await MainPage.DisplayAlert("Need Location", "Application will not work without Location permission!", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    if (!_serviceLoaded)
                    {
                        _serviceLoaded = true;
                        GeoFenceService.Initialize();
                        Debug.WriteLine("Service initialization complete");
                    }
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await MainPage.DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }

        private void Handle_TrailDefsUpdated(object sender, TrailDefsUpdatedArgs e)
        {
            FinishInitializations();
        }

        private void FinishInitializations()
        {
            RegisterOtherServices();
            RegisterViewModels();

            // tell the trail list it should build itself.
            var trailsVM = ViewModelLocator.Instance.TrailListViewModel;

            // Go to the first page which is the trail list
            trailsVM.BuildTrailListItems();
            NotificationService.Instance.NotifyNavigateToPage(PageManager.TrailListPageId, typeof(TrailList));

            // just for testing
            //AppStateService.Instance.ActiveTrailId = "GrotonPlace";
            CheckIfResumeMonitoringNeeded();
        }

        private void RegisterOtherServices()
        {          
        }

        private void RegisterViewModels()
        {
            // instantiate view models located in the view model locator
            ViewModelLocator.Instance.Initialize();
            PageManager.Instance.Clear();
        }

        private void CheckIfResumeMonitoringNeeded()
        {
            var currTrailId = AppStateService.Instance.ActiveTrailId;
            if (!string.IsNullOrEmpty(currTrailId))
            {
                NotificationService.Instance.NotifyResumeTrailMonitoring(currTrailId);
            }
        }
    }
}
