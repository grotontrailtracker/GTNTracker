using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.ViewModels
{
    public class PasswordVM : ViewModelBase
    {
        public string Title { set; get; }
        public string Prompt { set; get; }

        public string Password { set; get; }

        public bool Cancelled { get; set; }
    }

    public class PasswordEventArgs : EventArgs
    {
        public PasswordEventArgs(PasswordVM vm)
        {
            ViewModel = vm;
        }
        public PasswordVM ViewModel { get; set; }
    }
}
