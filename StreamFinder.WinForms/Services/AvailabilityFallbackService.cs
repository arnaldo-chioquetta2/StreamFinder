using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public class AvailabilityFallbackService
    {
        private readonly IniFileService iniFileService;
        private readonly List<IAvailabilityFallbackSource> sources;

        public AvailabilityFallbackService(
            IniFileService iniFileService,
            IEnumerable<IAvailabilityFallbackSource> sources)
        {
            this.iniFileService = iniFileService ?? throw new ArgumentNullException("iniFileService");
            this.sources = (sources ?? Enumerable.Empty<IAvailabilityFallbackSource>())
                .Where(source => source != null)
                .ToList();
        }

        public AvailabilityFallbackService(IniFileService iniFileService)
            : this(iniFileService, null)
        {
        }

        public List<MediaAvailability> Combine(
            IEnumerable<MediaAvailability> primary,
            IEnumerable<MediaAvailability> supplementary)
        {
            var combined = new List<MediaAvailability>();
            AddUniqueResults(combined, primary);
            AddUniqueResults(combined, supplementary);
            return combined;
        }

        public async Task<List<MediaAvailability>> SearchAvailabilityAsync(
            MediaItem media,
            string country,
            CancellationToken cancellationToken)
        {
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            var result = new List<MediaAvailability>();
            if (!iniFileService.LoadSettings().EnableScraping || sources.Count == 0)
            {
                return result;
            }

            foreach (var source in sources)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var sourceResults = await source.SearchAvailabilityAsync(
                        media,
                        country,
                        cancellationToken);
                    AddUniqueResults(result, sourceResults);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // A source failure must not prevent other registered sources.
                }
            }

            return result;
        }

        private static void AddUniqueResults(
            List<MediaAvailability> target,
            IEnumerable<MediaAvailability> sourceResults)
        {
            if (sourceResults == null)
            {
                return;
            }

            foreach (var availability in sourceResults)
            {
                if (!IsValidAvailability(availability))
                {
                    continue;
                }

                var key = CreateIdentity(availability);
                if (!target.Any(existing => string.Equals(
                    CreateIdentity(existing), key, StringComparison.OrdinalIgnoreCase)))
                {
                    target.Add(availability);
                }
            }
        }

        private static bool IsValidAvailability(MediaAvailability availability)
        {
            if (availability == null || availability.Provider == null ||
                string.IsNullOrWhiteSpace(availability.Provider.Name))
            {
                return false;
            }

            var watchUrl = string.IsNullOrWhiteSpace(availability.WatchUrl)
                ? availability.Provider.WatchUrl
                : availability.WatchUrl;
            return string.IsNullOrWhiteSpace(watchUrl) ||
                PublicWebUrlValidator.TryValidate(watchUrl, out _);
        }

        private static string CreateIdentity(MediaAvailability availability)
        {
            var provider = availability.Provider;
            var providerIdentity = string.IsNullOrWhiteSpace(provider.Id)
                ? provider.Name
                : provider.Id;
            var watchUrl = string.IsNullOrWhiteSpace(availability.WatchUrl)
                ? provider.WatchUrl
                : availability.WatchUrl;
            return string.Format(
                "{0}|{1}|{2}|{3}",
                providerIdentity ?? string.Empty,
                provider.Name ?? string.Empty,
                availability.AvailabilityType,
                watchUrl ?? string.Empty);
        }
    }
}
