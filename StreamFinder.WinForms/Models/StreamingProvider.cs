namespace StreamFinder.WinForms.Models
{
    public class StreamingProvider
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public bool IsPaid { get; set; }
        public bool IsEnabled { get; set; }
        public string WatchUrl { get; set; }
    }
}
