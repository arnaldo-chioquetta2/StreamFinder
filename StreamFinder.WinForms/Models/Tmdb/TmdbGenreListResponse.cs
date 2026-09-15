using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbGenreListResponse
    {
        [JsonProperty("genres")]
        public List<TmdbGenre> Genres { get; set; }
    }
}