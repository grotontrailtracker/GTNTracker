using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GTNTracker.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, args);
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
                return true;
            }

            return false;
        }
    }
}
