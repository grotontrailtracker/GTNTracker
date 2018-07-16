using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Models
{
    public class AppSettings
    {
        private bool _displayVisit = true;
        private bool _useMeters = true;
        private bool _allowCapture = false;
        private bool _developerMode = false;

        public bool DisplayVisitPopups
        {
            get => _displayVisit;
            set => _displayVisit = value;
        }

        public bool DisplayMeters
        {
            get => _useMeters;
            set => _useMeters = value;
        }

        public bool AllowCapture
        {
            get => _allowCapture;
            set => _allowCapture = value;
        }

        public bool DeveloperMode
        {
            get => _developerMode;
            set => _developerMode = value;
        }
    }
}
