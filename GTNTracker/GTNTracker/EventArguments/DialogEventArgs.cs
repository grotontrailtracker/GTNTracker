using System;
using GTNTracker.ViewModels;

namespace GTNTracker.EventArguments
{
    public class DialogEventArgs : EventArgs
    {
        public DialogEventArgs(ViewModelBase vm)
        {
            ViewModel = vm;
        }
        public ViewModelBase ViewModel { get; private set; }
    }
}
