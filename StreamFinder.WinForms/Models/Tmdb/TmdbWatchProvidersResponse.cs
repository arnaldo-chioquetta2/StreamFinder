using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbWatchProvidersResponse
    {
        [JsonProperty("results")]
        public Dictionary<string, TmdbWatchProviderCountryDto> Results { get; set; }
    }
}
