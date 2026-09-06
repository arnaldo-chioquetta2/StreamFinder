using System.Collections.Generic;

namespace StreamFinder.WinForms.Models
{
    public class MediaItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public int? Year { get; set; }
        public double? Rating { get; set; }
        public string PosterUrl { get; set; }
        public MediaType MediaType { get; set; }
        public List<string> Genres { get; set; }

        public MediaItem()
        {
            Genres = new List<string>();
        }
    }
}
