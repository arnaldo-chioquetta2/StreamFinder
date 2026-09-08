using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public interface IAvailabilityFallbackSource
    {
        string Name { get; }

        Task<List<MediaAvailability>> SearchAvailabilityAsync(
            MediaItem media,
            string country,
            CancellationToken cancellationToken);
    }
}
