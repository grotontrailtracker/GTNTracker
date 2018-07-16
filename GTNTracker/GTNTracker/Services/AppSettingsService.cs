using GTNTracker.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GTNTracker.Services
{
    public class AppSettingsService
    {
        private static AppSettingsService _instance;
        private ISettings _settingsPlugin;
        private AppSettings _appSettings = new AppSettings();
        private string _settingsName = "gtnAppSettings";

        public static AppSettingsService Instance => _instance ?? (_instance = new AppSettingsService());

        public AppSettings AppSettings
        {
            get => _appSettings;
        }

        public void UpdateAppSettings(AppSettings app = null)
        {
            var appSettingsToUpdate = (app != null) ? app : _appSettings;
            var data = JsonConvert.SerializeObject(appSettingsToUpdate);
            _settingsPlugin.AddOrUpdateValue(_settingsName, data);
            _appSettings = appSettingsToUpdate;
        }

        private AppSettingsService()
        {
            _settingsPlugin = CrossSettings.Current;
            Initialize();
        }

        private void Initialize()
        {
            var val = _settingsPlugin.GetValueOrDefault(_settingsName, string.Empty);
            if (!string.IsNullOrEmpty(val))
            {
                var obj = JsonConvert.DeserializeObject<AppSettings>(val);
                if (obj != null)
                {
                    _appSettings = obj;
                }
            }
        }
    }
}
