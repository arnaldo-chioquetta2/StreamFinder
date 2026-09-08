using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbSearchResponse
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        [JsonProperty("results")]
        public List<TmdbSearchResult> Results { get; set; }
    }
}
