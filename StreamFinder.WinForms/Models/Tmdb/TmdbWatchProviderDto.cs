using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbWatchProviderDto
    {
        [JsonProperty("provider_id")]
        public int ProviderId { get; set; }

        [JsonProperty("provider_name")]
        public string ProviderName { get; set; }

        [JsonProperty("logo_path")]
        public string LogoPath { get; set; }

        [JsonProperty("display_priority")]
        public int? DisplayPriority { get; set; }
    }
}
