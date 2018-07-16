using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Interfaces
{
    public interface IEmailService
    {
        void CreateEmail(List<string> emailAddresses, List<string> ccs, string subject, string body, string htmlBody);

        void EmailWaypoint(string waypointData, string filePath, int id);
    }
}
