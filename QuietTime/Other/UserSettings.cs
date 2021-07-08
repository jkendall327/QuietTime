using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace QuietTime.Other
{
    class UserSettings
    {
        private UserSettings() { }

        private static string? _jsonSource;
        private static UserSettings? _appSettings;
        public static UserSettings Default
        {
            get
            {
                if (_appSettings != null)
                {
                    return _appSettings;
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("usersettings.json", optional: false, reloadOnChange: true);

                _jsonSource = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}usersettings.json";

                var config = builder.Build();

                _appSettings = new UserSettings();
                config.Bind(_appSettings);

                return _appSettings;
            }
        }

        public void Save()
        {
            if (_jsonSource is null) return;

            var json = JsonSerializer.Serialize(_appSettings, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(_jsonSource, json);
        }

        public bool NotificationsEnabled { get; set; }
        public bool MinimizeOnClose { get; set; }
        public bool LaunchMinimized { get; set; }
        public bool LockOnStartup { get; set; }
        public int DefaultMaxVolume { get; set; }
    }
}
