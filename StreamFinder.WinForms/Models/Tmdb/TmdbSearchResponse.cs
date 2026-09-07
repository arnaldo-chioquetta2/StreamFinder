using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbSearchResponse
    {
        [JsonProperty("results")]
        public List<TmdbSearchResult> Results { get; set; }
    }
}
