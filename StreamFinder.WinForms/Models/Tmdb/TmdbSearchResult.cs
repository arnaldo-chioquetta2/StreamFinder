using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Models.Tmdb
{
    public class TmdbSearchResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonProperty("vote_average")]
        public double? VoteAverage { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("genre_ids")]
        public List<int> GenreIds { get; set; }
    }
}
