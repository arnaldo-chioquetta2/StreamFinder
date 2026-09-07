using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbWatchProviderCountryDto
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("flatrate")]
        public List<TmdbWatchProviderDto> Flatrate { get; set; }

        [JsonProperty("free")]
        public List<TmdbWatchProviderDto> Free { get; set; }

        [JsonProperty("ads")]
        public List<TmdbWatchProviderDto> Ads { get; set; }

        [JsonProperty("rent")]
        public List<TmdbWatchProviderDto> Rent { get; set; }

        [JsonProperty("buy")]
        public List<TmdbWatchProviderDto> Buy { get; set; }
    }
}
