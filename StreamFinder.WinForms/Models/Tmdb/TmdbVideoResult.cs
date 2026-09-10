using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbVideoResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("official")]
        public bool Official { get; set; }

        [JsonProperty("iso_639_1")]
        public string Language { get; set; }

        [JsonProperty("iso_3166_1")]
        public string Country { get; set; }

        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }
    }
}
