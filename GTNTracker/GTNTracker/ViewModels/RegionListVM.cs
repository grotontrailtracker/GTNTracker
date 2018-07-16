using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.ViewModels
{
    public class RegionListVM : ViewModelBase
    {
        private ObservableCollection<RegionSelectVM> _trailRegionList = new ObservableCollection<RegionSelectVM>();
        private RegionSelectVM _selectedRegion;

        public ObservableCollection<RegionSelectVM> RegionList
        {
            get => _trailRegionList;
            set => SetProperty(ref _trailRegionList, value);
        }

        public RegionSelectVM SelectedRegion
        {
            get => _selectedRegion;
            set => SetProperty(ref _selectedRegion, value);
        }

    }
}
