using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public class IniFileService
    {
        private const string DefaultCountry = "BR";
        private const bool DefaultEnableScraping = true;
        private const int DefaultCacheHours = 12;

        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string defaultValue,
            StringBuilder returnedValue,
            int size,
            string filePath);

        [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(
            string section,
            string key,
            string value,
            string filePath);

        public IniFileService()
        {
            ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            EnsureConfigFile();
        }

        public string ConfigFilePath { get; private set; }

        public string ReadString(string section, string key, string defaultValue)
        {
            try
            {
                var value = new StringBuilder(1024);
                GetPrivateProfileString(section, key, defaultValue, value, value.Capacity, ConfigFilePath);
                return value.ToString();
            }
            catch
            {
                return defaultValue;
            }
        }

        public void WriteString(string section, string key, string value)
        {
            try
            {
                WritePrivateProfileString(section, key, value ?? string.Empty, ConfigFilePath);
            }
            catch
            {
                // A configuration write failure must not close the application.
            }
        }

        public bool ReadBool(string section, string key, bool defaultValue)
        {
            bool value;
            return bool.TryParse(ReadString(section, key, defaultValue.ToString()), out value)
                ? value
                : defaultValue;
        }

        public void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, value.ToString().ToLowerInvariant());
        }

        public int ReadInt(string section, string key, int defaultValue)
        {
            int value;
            return int.TryParse(ReadString(section, key, defaultValue.ToString()), out value)
                ? value
                : defaultValue;
        }

        public void WriteInt(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
        }

        public AppSettings LoadSettings()
        {
            var country = ReadString("General", "Country", DefaultCountry);
            var cacheHours = ReadInt("General", "CacheHours", DefaultCacheHours);

            return new AppSettings
            {
                Country = string.IsNullOrWhiteSpace(country) ? DefaultCountry : country.Trim(),
                EnableScraping = ReadBool("General", "EnableScraping", DefaultEnableScraping),
                CacheHours = cacheHours > 0 ? cacheHours : DefaultCacheHours,
                TmdbApiKey = ReadString("Api", "TmdbApiKey", string.Empty),
                YouTubeApiKey = ReadString("Api", "YouTubeApiKey", string.Empty)
            };
        }

        public void SaveSettings(AppSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            WriteString("General", "Country", string.IsNullOrWhiteSpace(settings.Country) ? DefaultCountry : settings.Country.Trim());
            WriteBool("General", "EnableScraping", settings.EnableScraping);
            WriteInt("General", "CacheHours", settings.CacheHours > 0 ? settings.CacheHours : DefaultCacheHours);
            WriteString("Api", "TmdbApiKey", settings.TmdbApiKey ?? string.Empty);
            WriteString("Api", "YouTubeApiKey", settings.YouTubeApiKey ?? string.Empty);
        }

        public bool IsProviderEnabled(string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return false;
            }

            return ReadBool("Providers", providerId, false);
        }

        public void SetProviderEnabled(string providerId, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return;
            }

            WriteBool("Providers", providerId, enabled);
        }

        private void EnsureConfigFile()
        {
            if (File.Exists(ConfigFilePath))
            {
                return;
            }

            SaveSettings(new AppSettings
            {
                Country = DefaultCountry,
                EnableScraping = DefaultEnableScraping,
                CacheHours = DefaultCacheHours,
                TmdbApiKey = string.Empty,
                YouTubeApiKey = string.Empty
            });

            SetProviderEnabled("Netflix", true);
            SetProviderEnabled("PrimeVideo", true);
            SetProviderEnabled("DisneyPlus", false);
            SetProviderEnabled("Max", false);
            SetProviderEnabled("AppleTV", false);
            SetProviderEnabled("ParamountPlus", false);
            SetProviderEnabled("Globoplay", false);
            SetProviderEnabled("YouTube", true);
            SetProviderEnabled("PlutoTV", true);
            SetProviderEnabled("MercadoPlay", true);
        }
    }
}
