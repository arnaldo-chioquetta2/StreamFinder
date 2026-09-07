namespace StreamFinder.WinForms.Models
{
    public class AppSettings
    {
        public string Country { get; set; }
        public bool EnableScraping { get; set; }
        public int CacheHours { get; set; }
        public string TmdbApiKey { get; set; }
        public string YouTubeApiKey { get; set; }
    }
}
