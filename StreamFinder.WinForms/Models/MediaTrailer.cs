using System;

namespace StreamFinder.WinForms.Models
{
    public class MediaTrailer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Key { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsProbablyDubbed { get; set; }
        public string Type { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string WatchUrl { get; set; }
    }
}
