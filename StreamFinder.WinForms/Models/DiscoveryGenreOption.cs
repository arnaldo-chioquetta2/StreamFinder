namespace StreamFinder.WinForms.Models
{
    public class DiscoveryGenreOption
    {
        public string Name { get; set; }
        public int? MovieGenreId { get; set; }
        public int? TvGenreId { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}