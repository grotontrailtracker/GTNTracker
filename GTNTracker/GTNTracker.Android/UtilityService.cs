using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GTNTracker.Droid;
using GTNTracker.Interfaces;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(UtilityService))]
namespace GTNTracker.Droid
{

    public class UtilityService : IUtilityService
    {
        public bool DeleteFile(string path)
        {
            bool success = true;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"----->>>>>Deleted file: {path}");
                }
                else
                {
                    Console.WriteLine($"------>>>>File {path} does not exist!!!");
                    success = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"******Exception: {e.Message}");
                success = false;
            }

            return success;
        }
    }
}