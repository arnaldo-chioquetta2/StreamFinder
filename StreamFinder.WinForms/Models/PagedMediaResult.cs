using System.Collections.Generic;

namespace StreamFinder.WinForms.Models
{
    public class PagedMediaResult
    {
        public PagedMediaResult()
        {
            Items = new List<MediaItem>();
        }

        public List<MediaItem> Items { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }
}
