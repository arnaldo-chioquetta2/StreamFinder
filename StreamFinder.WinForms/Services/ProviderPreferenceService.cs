using System;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public class ProviderPreferenceService
    {
        private readonly IniFileService iniFileService;

        public ProviderPreferenceService(IniFileService iniFileService)
        {
            if (iniFileService == null)
            {
                throw new ArgumentNullException(nameof(iniFileService));
            }

            this.iniFileService = iniFileService;
        }

        public bool ShouldShowAvailability(MediaAvailability availability)
        {
            if (availability == null)
            {
                return false;
            }

            if (availability.AvailabilityType == AvailabilityType.Rent ||
                availability.AvailabilityType == AvailabilityType.Buy)
            {
                return true;
            }

            string providerKey;
            if (!TryGetProviderKey(availability.Provider == null ? null : availability.Provider.Name, out providerKey))
            {
                return true;
            }

            return iniFileService.IsProviderEnabled(providerKey);
        }

        public bool IsAvailabilityCompatibleWithOwnedServices(MediaAvailability availability)
        {
            if (availability == null ||
                availability.AvailabilityType == AvailabilityType.Rent ||
                availability.AvailabilityType == AvailabilityType.Buy)
            {
                return false;
            }

            string providerKey;
            return TryGetProviderKey(availability.Provider == null ? null : availability.Provider.Name, out providerKey) &&
                iniFileService.IsProviderEnabled(providerKey);
        }

        public bool TryGetProviderKey(string providerName, out string providerKey)
        {
            providerKey = null;
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return false;
            }

            var name = providerName.Trim();
            if (name.Equals("Netflix", StringComparison.OrdinalIgnoreCase))
            {
                providerKey = "Netflix";
            }
            else if (name.IndexOf("prime video", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("amazon video", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "PrimeVideo";
            }
            else if (name.IndexOf("disney", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "DisneyPlus";
            }
            else if (name.Equals("Max", StringComparison.OrdinalIgnoreCase) ||
                name.IndexOf("hbo max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "Max";
            }
            else if (name.IndexOf("apple tv", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "AppleTV";
            }
            else if (name.IndexOf("paramount", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "ParamountPlus";
            }
            else if (name.IndexOf("globoplay", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "Globoplay";
            }
            else if (name.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
            {
                providerKey = "YouTube";
            }
            else if (name.IndexOf("pluto tv", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "PlutoTV";
            }
            else if (name.IndexOf("mercado play", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                providerKey = "MercadoPlay";
            }

            return providerKey != null;
        }
    }
}
