﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using GTNTracker.Services;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class TrailListItemVM : ViewModelBase
    {
        bool _isCompleted;
        string _name;
        string _trailId;
        DateTime? _dateCompleted;
        int _numberEntered;
        int _numberDestinations;
        TrailContentViewModel _viewModel;
        bool _isStarted;
        double _progress;
        ImageSource _trailImage;
        bool _showImage;
        bool _isViewable = true;
        ICommand _tapCommand;

        public TrailListItemVM()
        {
            TapCommand = new Command(HandleTapCommand);
        }

        public ICommand TapCommand
        {
            get => _tapCommand;
            set => SetProperty(ref _tapCommand, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string TrailId
        {
            get => _trailId;
            set => SetProperty(ref _trailId, value); 
        }

        public bool IsStarted
        {
            get => _isStarted;
            set => SetProperty(ref _isStarted, value);
        }

        public bool IsViewable
        {
            get => _isViewable;
            set => SetProperty(ref _isViewable, value);
        }

        public bool Completed
        {
            get => _isCompleted;
            set
            {
                SetProperty(ref _isCompleted, value);
                NotifyPropertyChanged("InProgress");
            }
        }

        public bool InProgress
        {
            get => !_isCompleted;
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public DateTime? DateCompleted
        {
            get => _dateCompleted;
            set => SetProperty(ref _dateCompleted, value);
        }

        public int NumberDestinations
        {
            get => _numberDestinations;
            set => SetProperty(ref _numberDestinations, value);
        }

        public int NumberEntered
        {
            get => _numberEntered;
            set => SetProperty(ref _numberEntered, value);
        }

        public ImageSource Image
        {
            get => _trailImage;
            set => SetProperty(ref _trailImage, value);
        }

        public bool ShowImage
        {
            get => _showImage;
            set => SetProperty(ref _showImage, value);
        }

        public ContentPage TrailPage { get; set; }

        public TrailContentViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                if (_viewModel != null)
                {
                    NumberEntered = _viewModel.NumberEntered;
                    NumberDestinations = _viewModel.NumberDestinations;
                    Completed = _viewModel.IsTrailListComplete;
                    UpdateProgress();

                    _viewModel.PropertyChanged += _viewModel_PropertyChanged;
                }
            }
        }

        private void UpdateProgress()
        {
            if (NumberDestinations > 0)
            {
                double entered = NumberEntered;
                double total = NumberDestinations;
                var percent = entered / total;
                Progress = percent;
            }
            else
            {
                Progress = 0;
            }
        }

        private void HandleTapCommand()
        {
            if (Image != null)
            {
                NotificationService.Instance.NotifyZoomImage(Image);
            }
        }

        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            switch (propName)
            {
                case "NumberDestinations":
                    NumberDestinations = _viewModel.NumberDestinations;
                    break;
                case "NumberEntered":
                    NumberEntered = _viewModel.NumberEntered;
                    UpdateProgress();
                    break;
                case "IsTrailListComplete":
                    Completed = _viewModel.IsTrailListComplete;
                    //DateCompleted = _viewModel.DateCompleted;
                    break;
                case "DateCompleted":
                    DateCompleted = _viewModel.DateCompleted;
                    break;
                case "IsStarted":
                    IsStarted = _viewModel.IsStarted;
                    break;
                case "IsViewable":
                    IsViewable = _viewModel.IsViewable;
                    break;
                default:
                    break;
            }
        }
    }
}
