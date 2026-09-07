namespace StreamFinder.WinForms.Models
{
    public class MediaAvailability
    {
        public string MediaId { get; set; }
        public StreamingProvider Provider { get; set; }
        public AvailabilityType AvailabilityType { get; set; }
        public string WatchUrl { get; set; }
    }
}
