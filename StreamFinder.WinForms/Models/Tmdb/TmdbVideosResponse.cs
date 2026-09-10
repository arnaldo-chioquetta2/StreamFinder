using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbVideosResponse
    {
        [JsonProperty("results")]
        public List<TmdbVideoResult> Results { get; set; }
    }
}
