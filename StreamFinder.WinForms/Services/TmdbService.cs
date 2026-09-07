using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamFinder.WinForms.Models;
using StreamFinder.WinForms.Models.Tmdb;

namespace StreamFinder.WinForms.Services
{
    public class TmdbService
    {
        private const string ApiBaseUrl = "https://api.themoviedb.org/3/search/multi";
        private const string DetailsBaseUrl = "https://api.themoviedb.org/3";
        private const string PosterBaseUrl = "https://image.tmdb.org/t/p/w342";
        private const string ProviderLogoBaseUrl = "https://image.tmdb.org/t/p/w92";

        private readonly HttpService httpService;
        private readonly IniFileService iniFileService;
        private readonly CacheService cacheService;

        public TmdbService()
            : this(new HttpService(), new IniFileService(), new CacheService())
        {
        }

        public TmdbService(HttpService httpService, IniFileService iniFileService)
            : this(httpService, iniFileService, new CacheService())
        {
        }

        public TmdbService(HttpService httpService, IniFileService iniFileService, CacheService cacheService)
        {
            this.httpService = httpService ?? throw new ArgumentNullException("httpService");
            this.iniFileService = iniFileService ?? throw new ArgumentNullException("iniFileService");
            this.cacheService = cacheService ?? throw new ArgumentNullException("cacheService");
        }

        public async Task<List<MediaItem>> SearchAsync(string query)
        {
            var results = new List<MediaItem>();
            if (string.IsNullOrWhiteSpace(query))
            {
                return results;
            }

            var settings = iniFileService.LoadSettings();
            if (string.IsNullOrWhiteSpace(settings.TmdbApiKey))
            {
                throw new InvalidOperationException("Configure sua chave do TMDb em Configura\u00e7\u00f5es.");
            }

            var normalizedQuery = query.Trim();
            var cacheKey = string.Format(
                CultureInfo.InvariantCulture,
                "tmdb-search-multi|{0}|pt-BR|BR|1",
                normalizedQuery.ToLowerInvariant());

            string json;
            if (!cacheService.TryGet(cacheKey, settings.CacheHours, out json))
            {
                var url = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}?api_key={1}&language=pt-BR&region=BR&page=1&query={2}",
                    ApiBaseUrl,
                    Uri.EscapeDataString(settings.TmdbApiKey),
                    Uri.EscapeDataString(normalizedQuery));

                json = await httpService.GetJsonAsync(url).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        cacheService.Set(cacheKey, json);
                    }
                    catch (CachePersistenceException)
                    {
                        // A cache write failure must not invalidate a valid API response.
                    }
                }
            }

            var response = JsonConvert.DeserializeObject<TmdbSearchResponse>(json);
            if (response == null || response.Results == null)
            {
                return results;
            }

            foreach (var result in response.Results)
            {
                var item = ConvertResult(result);
                if (item != null)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<MediaItem> GetDetailsAsync(MediaItem media)
        {
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            int mediaId;
            if (!int.TryParse(media.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out mediaId) || mediaId <= 0)
            {
                throw new ArgumentException("O ID do item de mÃ­dia Ã© invÃ¡lido.", "media");
            }

            string mediaType;
            if (media.MediaType == MediaType.Movie)
            {
                mediaType = "movie";
            }
            else if (media.MediaType == MediaType.Tv)
            {
                mediaType = "tv";
            }
            else
            {
                throw new ArgumentException("O tipo do item de mÃ­dia Ã© invÃ¡lido.", "media");
            }

            var settings = iniFileService.LoadSettings();
            if (string.IsNullOrWhiteSpace(settings.TmdbApiKey))
            {
                throw new InvalidOperationException("Configure sua chave do TMDb em ConfiguraÃ§Ãµes.");
            }

            var cacheKey = string.Format(
                CultureInfo.InvariantCulture,
                "tmdb-details-{0}-{1}-pt-BR",
                mediaType,
                mediaId);

            string json;
            if (!cacheService.TryGet(cacheKey, settings.CacheHours, out json))
            {
                var url = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}/{2}?api_key={3}&language=pt-BR",
                    DetailsBaseUrl,
                    mediaType,
                    mediaId,
                    Uri.EscapeDataString(settings.TmdbApiKey));

                json = await httpService.GetJsonAsync(url).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        cacheService.Set(cacheKey, json);
                    }
                    catch (CachePersistenceException)
                    {
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException("A resposta de detalhes do TMDb estÃ¡ vazia.");
            }

            var details = JsonConvert.DeserializeObject<TmdbDetailsResponse>(json);
            if (details == null)
            {
                throw new InvalidOperationException("A resposta de detalhes do TMDb Ã© invÃ¡lida.");
            }

            return ConvertDetails(media, details);
        }

        public async Task<List<MediaAvailability>> GetWatchProvidersAsync(MediaItem media)
        {
            var availability = new List<MediaAvailability>();
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            int mediaId;
            if (!int.TryParse(media.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out mediaId) || mediaId <= 0)
            {
                throw new ArgumentException("O ID do item de mÃ­dia Ã© invÃ¡lido.", "media");
            }

            string mediaType;
            if (media.MediaType == MediaType.Movie)
            {
                mediaType = "movie";
            }
            else if (media.MediaType == MediaType.Tv)
            {
                mediaType = "tv";
            }
            else
            {
                throw new ArgumentException("O tipo do item de mÃ­dia Ã© invÃ¡lido.", "media");
            }

            var settings = iniFileService.LoadSettings();
            if (string.IsNullOrWhiteSpace(settings.TmdbApiKey))
            {
                throw new InvalidOperationException("Configure sua chave do TMDb em ConfiguraÃ§Ãµes.");
            }

            var country = NormalizeCountry(settings.Country);
            var cacheKey = string.Format(
                CultureInfo.InvariantCulture,
                "tmdb-watchproviders-{0}-{1}-{2}",
                mediaType,
                mediaId,
                country);

            string json;
            if (!cacheService.TryGet(cacheKey, settings.CacheHours, out json))
            {
                var url = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}/{2}/watch/providers?api_key={3}",
                    DetailsBaseUrl,
                    mediaType,
                    mediaId,
                    Uri.EscapeDataString(settings.TmdbApiKey));

                json = await httpService.GetJsonAsync(url).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        cacheService.Set(cacheKey, json);
                    }
                    catch (CachePersistenceException)
                    {
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return availability;
            }

            var response = JsonConvert.DeserializeObject<TmdbWatchProvidersResponse>(json);
            if (response == null || response.Results == null)
            {
                return availability;
            }

            TmdbWatchProviderCountryDto countryResult;
            if (!response.Results.TryGetValue(country, out countryResult) || countryResult == null)
            {
                return availability;
            }

            AddAvailabilities(availability, media.Id, countryResult, countryResult.Flatrate, AvailabilityType.Subscription, true);
            AddAvailabilities(availability, media.Id, countryResult, countryResult.Free, AvailabilityType.Free, false);
            AddAvailabilities(availability, media.Id, countryResult, countryResult.Ads, AvailabilityType.Ads, false);
            AddAvailabilities(availability, media.Id, countryResult, countryResult.Rent, AvailabilityType.Rent, true);
            AddAvailabilities(availability, media.Id, countryResult, countryResult.Buy, AvailabilityType.Buy, true);
            return availability;
        }

        private static string NormalizeCountry(string country)
        {
            var normalized = (country ?? string.Empty).Trim().ToUpperInvariant();
            if (normalized.Length != 2 || normalized[0] < 'A' || normalized[0] > 'Z' || normalized[1] < 'A' || normalized[1] > 'Z')
            {
                return "BR";
            }

            return normalized;
        }

        private static void AddAvailabilities(
            List<MediaAvailability> target,
            string mediaId,
            TmdbWatchProviderCountryDto country,
            List<TmdbWatchProviderDto> providers,
            AvailabilityType availabilityType,
            bool isPaid)
        {
            if (providers == null)
            {
                return;
            }

            foreach (var provider in providers)
            {
                if (provider == null || provider.ProviderId <= 0 || string.IsNullOrWhiteSpace(provider.ProviderName))
                {
                    continue;
                }

                var providerId = provider.ProviderId.ToString(CultureInfo.InvariantCulture);
                var duplicate = false;
                foreach (var existing in target)
                {
                    if (existing.AvailabilityType == availabilityType &&
                        existing.Provider != null &&
                        existing.Provider.Id == providerId &&
                        existing.WatchUrl == country.Link)
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate)
                {
                    continue;
                }

                target.Add(new MediaAvailability
                {
                    MediaId = mediaId,
                    AvailabilityType = availabilityType,
                    WatchUrl = country.Link,
                    Provider = new StreamingProvider
                    {
                        Id = providerId,
                        Name = provider.ProviderName,
                        LogoUrl = string.IsNullOrWhiteSpace(provider.LogoPath) ? null : ProviderLogoBaseUrl + provider.LogoPath,
                        WatchUrl = country.Link,
                        IsPaid = isPaid,
                        IsEnabled = true
                    }
                });
            }
        }

        private static MediaItem ConvertResult(TmdbSearchResult result)
        {
            if (result == null || result.Id <= 0)
            {
                return null;
            }

            MediaType mediaType;
            string title;
            string originalTitle;
            string date;

            if (string.Equals(result.MediaType, "movie", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = MediaType.Movie;
                title = result.Title;
                originalTitle = result.OriginalTitle;
                date = result.ReleaseDate;
            }
            else if (string.Equals(result.MediaType, "tv", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = MediaType.Tv;
                title = result.Name;
                originalTitle = result.OriginalName;
                date = result.FirstAirDate;
            }
            else
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            return new MediaItem
            {
                Id = result.Id.ToString(CultureInfo.InvariantCulture),
                Title = title,
                OriginalTitle = originalTitle,
                Overview = result.Overview,
                Year = ParseYear(date),
                Rating = result.VoteAverage,
                PosterUrl = string.IsNullOrWhiteSpace(result.PosterPath)
                    ? null
                    : PosterBaseUrl + result.PosterPath,
                MediaType = mediaType
            };
        }

        private static int? ParseYear(string date)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate)
                ? parsedDate.Year
                : (int?)null;
        }

        private static MediaItem ConvertDetails(MediaItem original, TmdbDetailsResponse details)
        {
            var title = original.MediaType == MediaType.Movie ? details.Title : details.Name;
            var originalTitle = original.MediaType == MediaType.Movie ? details.OriginalTitle : details.OriginalName;
            var date = original.MediaType == MediaType.Movie ? details.ReleaseDate : details.FirstAirDate;

            return new MediaItem
            {
                Id = original.Id,
                MediaType = original.MediaType,
                Title = string.IsNullOrWhiteSpace(title) ? original.Title : title,
                OriginalTitle = string.IsNullOrWhiteSpace(originalTitle) ? original.OriginalTitle : originalTitle,
                Overview = string.IsNullOrWhiteSpace(details.Overview) ? original.Overview : details.Overview,
                Year = ParseYear(date) ?? original.Year,
                Rating = details.VoteAverage ?? original.Rating,
                PosterUrl = string.IsNullOrWhiteSpace(details.PosterPath)
                    ? original.PosterUrl
                    : PosterBaseUrl + details.PosterPath,
                Genres = ConvertGenres(original.Genres, details.Genres)
            };
        }

        private static List<string> ConvertGenres(List<string> originalGenres, List<TmdbGenre> genres)
        {
            var converted = new List<string>();
            if (genres != null)
            {
                foreach (var genre in genres)
                {
                    if (genre != null && !string.IsNullOrWhiteSpace(genre.Name))
                    {
                        converted.Add(genre.Name);
                    }
                }
            }

            if (converted.Count > 0)
            {
                return converted;
            }

            return originalGenres == null ? new List<string>() : new List<string>(originalGenres);
        }
    }
}
