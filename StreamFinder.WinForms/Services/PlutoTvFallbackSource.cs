using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public sealed class PlutoTvFallbackSource : IAvailabilityFallbackSource
    {
        private const string SearchUrl = "https://pluto.tv/br/search?query={0}";

        private readonly HttpService httpService;
        private readonly IniFileService iniFileService;

        public PlutoTvFallbackSource(HttpService httpService, IniFileService iniFileService)
        {
            this.httpService = httpService ?? throw new ArgumentNullException("httpService");
            this.iniFileService = iniFileService ?? throw new ArgumentNullException("iniFileService");
        }

        public string Name
        {
            get { return "Pluto TV"; }
        }

        public async Task<List<MediaAvailability>> SearchAvailabilityAsync(
            MediaItem media,
            string country,
            CancellationToken cancellationToken)
        {
            var result = new List<MediaAvailability>();
            if (media == null || !string.Equals((country ?? string.Empty).Trim(), "BR", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            var titleCandidates = GetTitleCandidates(media);
            if (titleCandidates.Count == 0)
            {
                return result;
            }

            var queryTitle = string.IsNullOrWhiteSpace(media.Title)
                ? media.OriginalTitle
                : media.Title;
            if (string.IsNullOrWhiteSpace(queryTitle))
            {
                return result;
            }

            var searchUrl = string.Format(
                CultureInfo.InvariantCulture,
                SearchUrl,
                Uri.EscapeDataString(queryTitle.Trim()));
            Uri validatedSearchUrl;
            if (!PublicWebUrlValidator.TryValidate(searchUrl, out validatedSearchUrl))
            {
                return result;
            }

            var html = await httpService.GetStringAsync(validatedSearchUrl.AbsoluteUri, cancellationToken)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var link in ExtractContentLinks(html, media.MediaType))
            {
                if (!MatchesTitle(link.Text, titleCandidates) ||
                    !MatchesYear(link.Text, media.Year))
                {
                    continue;
                }

                result.Add(new MediaAvailability
                {
                    MediaId = media.Id,
                    AvailabilityType = AvailabilityType.Ads,
                    WatchUrl = link.Url,
                    Provider = new StreamingProvider
                    {
                        Id = "pluto-tv",
                        Name = "Pluto TV",
                        IsPaid = false,
                        IsEnabled = iniFileService.IsProviderEnabled("PlutoTV"),
                        WatchUrl = link.Url,
                        LogoUrl = null
                    }
                });
                break;
            }

            return result;
        }

        private static List<string> GetTitleCandidates(MediaItem media)
        {
            var candidates = new List<string>();
            AddTitleCandidate(candidates, media.Title);
            AddTitleCandidate(candidates, media.OriginalTitle);
            return candidates;
        }

        private static void AddTitleCandidate(List<string> candidates, string value)
        {
            var normalized = NormalizeText(value);
            if (!string.IsNullOrWhiteSpace(normalized) && !candidates.Contains(normalized))
            {
                candidates.Add(normalized);
            }
        }

        private static List<PublicContentLink> ExtractContentLinks(string html, MediaType mediaType)
        {
            var links = new List<PublicContentLink>();
            if (string.IsNullOrWhiteSpace(html))
            {
                return links;
            }

            var expectedSegment = mediaType == MediaType.Movie
                ? "/br/on-demand/movies/"
                : "/br/on-demand/series/";
            var position = 0;
            while (position < html.Length)
            {
                var anchorStart = html.IndexOf("<a", position, StringComparison.OrdinalIgnoreCase);
                if (anchorStart < 0)
                {
                    break;
                }

                var anchorEnd = html.IndexOf(">", anchorStart);
                if (anchorEnd < 0)
                {
                    break;
                }

                var closeAnchor = html.IndexOf("</a>", anchorEnd, StringComparison.OrdinalIgnoreCase);
                if (closeAnchor < 0)
                {
                    break;
                }

                var href = ExtractAttribute(html.Substring(anchorStart, anchorEnd - anchorStart + 1), "href");
                var text = StripTags(html.Substring(anchorEnd + 1, closeAnchor - anchorEnd - 1));
                Uri contentUri;
                if (TryCreateContentUri(href, expectedSegment, out contentUri))
                {
                    links.Add(new PublicContentLink
                    {
                        Url = contentUri.AbsoluteUri,
                        Text = text
                    });
                }

                position = closeAnchor + 4;
            }

            return links;
        }

        private static bool TryCreateContentUri(string href, string expectedSegment, out Uri uri)
        {
            uri = null;
            if (string.IsNullOrWhiteSpace(href))
            {
                return false;
            }

            Uri candidate;
            if (!Uri.TryCreate(href, UriKind.Absolute, out candidate))
            {
                if (!Uri.TryCreate("https://pluto.tv" + href, UriKind.Absolute, out candidate))
                {
                    return false;
                }
            }

            if (!PublicWebUrlValidator.TryValidate(candidate.AbsoluteUri, out candidate) ||
                !string.Equals(candidate.Host, "pluto.tv", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(candidate.Host, "www.pluto.tv", StringComparison.OrdinalIgnoreCase) ||
                candidate.AbsolutePath.IndexOf(expectedSegment, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return false;
            }

            uri = candidate;
            return true;
        }

        private static string ExtractAttribute(string tag, string attributeName)
        {
            var marker = attributeName + "=";
            var start = tag.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return null;
            }

            start += marker.Length;
            while (start < tag.Length && char.IsWhiteSpace(tag[start]))
            {
                start++;
            }

            if (start >= tag.Length)
            {
                return null;
            }

            var quote = tag[start] == '\'' || tag[start] == '"' ? tag[start++] : '\0';
            var end = quote == '\0'
                ? tag.IndexOfAny(new[] { ' ', '>' }, start)
                : tag.IndexOf(quote, start);
            if (end < 0)
            {
                end = tag.Length;
            }

            return WebUtility.HtmlDecode(tag.Substring(start, end - start));
        }

        private static string StripTags(string value)
        {
            var builder = new StringBuilder();
            var inTag = false;
            foreach (var character in WebUtility.HtmlDecode(value ?? string.Empty))
            {
                if (character == '<')
                {
                    inTag = true;
                }
                else if (character == '>')
                {
                    inTag = false;
                    builder.Append(' ');
                }
                else if (!inTag)
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        private static bool MatchesTitle(string text, IList<string> candidates)
        {
            var normalizedText = NormalizeText(text);
            foreach (var candidate in candidates)
            {
                if (string.Equals(normalizedText, candidate, StringComparison.OrdinalIgnoreCase) ||
                    normalizedText.StartsWith(candidate + " ", StringComparison.OrdinalIgnoreCase) &&
                    IsYearSuffix(normalizedText.Substring(candidate.Length + 1)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsYearSuffix(string value)
        {
            int year;
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out year) &&
                year >= 1800 && year <= 2100;
        }

        private static bool MatchesYear(string text, int? expectedYear)
        {
            if (!expectedYear.HasValue)
            {
                return true;
            }

            var normalizedText = NormalizeText(text);
            return normalizedText.IndexOf(expectedYear.Value.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) >= 0;
        }

        private static string NormalizeText(string value)
        {
            var builder = new StringBuilder();
            foreach (var character in (value ?? string.Empty).Normalize().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                }
                else
                {
                    builder.Append(' ');
                }
            }

            return string.Join(" ", builder.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private sealed class PublicContentLink
        {
            public string Url { get; set; }
            public string Text { get; set; }
        }
    }
}
