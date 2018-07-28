using GTNTracker.EventArguments;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class TrailListViewModel : ViewModelBase
    {
        private ObservableCollection<TrailListItemVM> _trailList = new ObservableCollection<TrailListItemVM>();
        private bool _areAllCompleted;
        private DateTime? _completeTime;
        private int _numberTrails;
        private int _numberDisplayableTrails;

        public TrailListViewModel()
        {
            BuildTrailListItems();
            NotificationService.Instance.ResumeMonitoring += HandleResumeTrailMonitoring;

            MessagingCenter.Subscribe<UIStopAllMonitoringRegions>(this, UIStopAllMonitoringRegions.MessageString,
                (sender) => { HandleUIStopAllMonitoringRegions(); });
        }

        public bool AllTrailsComplete
        {
            get => _areAllCompleted;
            set => SetProperty(ref _areAllCompleted, value);
        }

        public int NumberTrailsDisplay
        {
            get => _numberDisplayableTrails;
            set => SetProperty(ref _numberDisplayableTrails, value);
        }

        public ObservableCollection<TrailListItemVM> TrailList
        {
            get => _trailList;
            set => SetProperty(ref _trailList, value);
        }

        private void BuildTrailListItems()
        {
            var visitService = TrailVisitService.Instance;
            var trailDefService = TrailDefService.Instance;
            _numberTrails = trailDefService.TrailDefinitions.Count();
            if (!AppSettingsService.Instance.AppSettings.DeveloperMode)
            {
                NumberTrailsDisplay = trailDefService.TrailDefinitions.Count(t => !t.DeveloperMode);
            }
            else
            {
                NumberTrailsDisplay = _numberTrails;
            }

            foreach (var trailDef in trailDefService.TrailDefinitions.OrderBy(t => t.Identifier))
            {
                if (trailDef.DeveloperMode && !AppSettingsService.Instance.AppSettings.DeveloperMode)
                {
                    continue;
                }

                TrailListItemVM item = null;
                if (TrailDefService.Instance.GetRegionDefinition(trailDef.Identifier).Any())
                {
                    var visits = visitService.GetVisits(trailDef.Identifier);
                    var vm = new TrailContentViewModel();
                    vm.TrailDescription = trailDef.Description;
                    vm.Initialize(trailDef.Identifier, trailDef.Name, TrailDefService.Instance.GetRegionDefinition(trailDef.Identifier), visits);
                    item = new TrailListItemVM()
                    {
                        Name = trailDef.Name,
                        TrailId = trailDef.Identifier,
                        Completed = false,
                        TrailPage = new TrailContentPage(),
                        ViewModel = vm
                    };
                    item.PropertyChanged += TrailContentPropertyChanged;
                    item.TrailPage.BindingContext = item.ViewModel;
                    item.IsStarted = vm.IsStarted;

                    if (vm.NumberEntered == vm.NumberDestinations)
                    {
                        item.Completed = true;
                        item.DateCompleted = vm.RegionList.Max(d => d.DateCompleted);   // assume the last date timestamp is the trail complete time!
                    }
                }
                else
                {
                    // this should only be done for the test under construction trails, not in real app!
                    item = new TrailListItemVM()
                    {
                        Name = trailDef.Name,
                        Completed = true,
                        DateCompleted = DateTime.Now,
                        TrailPage = new UnderConstruction(),
                        ViewModel = null
                    };
                    item.TrailPage.Title = trailDef.Name;
                }
                TrailList.Add(item);
            }

            AllTrailsComplete = !TrailList.Any(t => !t.Completed);
        }

        private void TrailContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            if (propName == "Completed")
            {
                var vm = sender as TrailListItemVM;
                if (vm != null)
                {
                    // ignore this while we're building the list, otherwise it'll keep ringing off
                    if (TrailList.Count() == _numberTrails)
                    {
                        AllTrailsComplete = !TrailList.Any(t => !t.Completed);
                        if (AllTrailsComplete)
                        {
                            _completeTime = DateTime.Now;
                        }
                    }
                }
            }
        }

        private void HandleUIStopAllMonitoringRegions()
        {
            var trailVM = _trailList.FirstOrDefault(t => t.IsStarted);
            var contentVM = trailVM.ViewModel;
            contentVM.Stop();
        }

        private void HandleResumeTrailMonitoring(object sender, ResumeTrailMonitoringArgs e)
        {
            var activeTrailId = e.TrailId;
            if (!string.IsNullOrEmpty(activeTrailId))
            {
                // find the view model for the trail
                var activeTrailListVM = TrailList.FirstOrDefault(trail => trail.TrailId == activeTrailId);
                if (activeTrailListVM != null)
                {
                    var trailVM = activeTrailListVM.ViewModel;
                    if (!trailVM.IsTrailListComplete)
                    {
                        trailVM.Start();
                    }
                }
            }
        }
    }
}
