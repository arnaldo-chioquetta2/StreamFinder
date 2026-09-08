using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamFinder.WinForms.Models;
using StreamFinder.WinForms.Services;

namespace StreamFinder.WinForms
{
    public partial class MediaDetailsForm : Form
    {
        private MediaItem media;
        private readonly FavoritesService favoritesService;
        private readonly TmdbService tmdbService;
        private readonly IniFileService iniFileService;
        private readonly ProviderPreferenceService providerPreferenceService;
        private readonly AvailabilityFallbackService availabilityFallbackService;
        private readonly HttpService httpService;
        private CancellationTokenSource posterCancellation;
        private CancellationTokenSource providersCancellation;
        private readonly List<Image> providerImages = new List<Image>();
        private string currentPosterUrl;

        public MediaDetailsForm(
            MediaItem media,
            FavoritesService favoritesService,
            TmdbService tmdbService,
            IniFileService iniFileService,
            HttpService httpService,
            AvailabilityFallbackService availabilityFallbackService)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            if (favoritesService == null)
            {
                throw new ArgumentNullException(nameof(favoritesService));
            }

            if (tmdbService == null)
            {
                throw new ArgumentNullException(nameof(tmdbService));
            }

            if (iniFileService == null)
            {
                throw new ArgumentNullException(nameof(iniFileService));
            }

            if (httpService == null)
            {
                throw new ArgumentNullException(nameof(httpService));
            }

            if (availabilityFallbackService == null)
            {
                throw new ArgumentNullException(nameof(availabilityFallbackService));
            }

            this.media = media;
            this.favoritesService = favoritesService;
            this.tmdbService = tmdbService;
            this.iniFileService = iniFileService;
            providerPreferenceService = new ProviderPreferenceService(iniFileService);
            this.httpService = httpService;
            this.availabilityFallbackService = availabilityFallbackService;
            InitializeComponent();
            PopulateMediaDetails();
            UpdateFavoriteButton();
            SetPlaceholder();

            StartPosterLoading(media.PosterUrl);

            _ = EnrichDetailsAsync();
            _ = LoadWatchProvidersAsync();
        }

        private void PopulateMediaDetails()
        {
            lblTitle.Text = string.IsNullOrWhiteSpace(media.Title) ? "-" : media.Title;
            lblOriginalTitleValue.Text = string.IsNullOrWhiteSpace(media.OriginalTitle) ? "-" : media.OriginalTitle;
            lblYearValue.Text = media.Year.HasValue ? media.Year.Value.ToString(CultureInfo.InvariantCulture) : "-";
            lblRatingValue.Text = media.Rating.HasValue
                ? media.Rating.Value.ToString("0.0", CultureInfo.GetCultureInfo("pt-BR"))
                : "-";
            lblTypeValue.Text = media.MediaType == MediaType.Movie ? "Filme" : "Série";
            lblGenresValue.Text = media.Genres == null || media.Genres.Count == 0
                ? "-"
                : string.Join(", ", media.Genres);
            txtOverview.Text = string.IsNullOrWhiteSpace(media.Overview)
                ? "Sinopse não disponível."
                : media.Overview;
        }

        private async Task EnrichDetailsAsync()
        {
            try
            {
                var enrichedMedia = await tmdbService.GetDetailsAsync(media);
                if (IsDisposed || Disposing || enrichedMedia == null)
                {
                    return;
                }

                media = enrichedMedia;
                PopulateMediaDetails();
                UpdateFavoriteButton();
                UpdatePoster(enrichedMedia.PosterUrl);
            }
            catch (Exception)
            {
                // Existing data remains usable when enrichment is unavailable.
            }
        }

        private async Task LoadWatchProvidersAsync()
        {
            providersCancellation = new CancellationTokenSource();
            var cancellationToken = providersCancellation.Token;
            lblAvailabilityStatus.Text = "Buscando disponibilidade...";

            try
            {
                List<MediaAvailability> tmdbAvailability;
                try
                {
                    tmdbAvailability = await tmdbService.GetWatchProvidersAsync(media);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // A falha do TMDb não deve ser mascarada pelo fallback.
                    ShowAvailabilityErrorIfAvailable();
                    return;
                }

                if (cancellationToken.IsCancellationRequested || IsDisposed || Disposing)
                {
                    return;
                }

                var combinedAvailability = tmdbAvailability ?? new List<MediaAvailability>();
                var settings = iniFileService.LoadSettings();
                if (settings.EnableScraping && ShouldUseFallback(combinedAvailability))
                {
                    try
                    {
                        var fallbackAvailability = await availabilityFallbackService.SearchAvailabilityAsync(
                            media,
                            settings.Country,
                            cancellationToken);
                        combinedAvailability = availabilityFallbackService.Combine(
                            combinedAvailability,
                            fallbackAvailability);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        // Resultados TMDb válidos continuam sendo exibidos.
                    }
                }

                if (cancellationToken.IsCancellationRequested || IsDisposed || Disposing)
                {
                    return;
                }

                var filteredAvailability = FilterWatchProviders(combinedAvailability);
                RenderWatchProviders(
                    filteredAvailability,
                    combinedAvailability.Count > 0,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                ShowAvailabilityErrorIfAvailable();
            }
        }

        private static bool ShouldUseFallback(IList<MediaAvailability> availability)
        {
            if (availability == null || availability.Count == 0)
            {
                return true;
            }

            foreach (var item in availability)
            {
                if (item != null &&
                    (item.AvailabilityType == AvailabilityType.Subscription ||
                     item.AvailabilityType == AvailabilityType.Free ||
                     item.AvailabilityType == AvailabilityType.Ads))
                {
                    return false;
                }
            }

            return true;
        }

        private void ShowAvailabilityErrorIfAvailable()
        {
            if (!IsDisposed && !Disposing)
            {
                DisposeProviderControls();
                lblAvailabilityStatus.Text = "Não foi possível consultar a disponibilidade no momento.";
            }
        }

        private List<MediaAvailability> FilterWatchProviders(IList<MediaAvailability> availability)
        {
            var filtered = new List<MediaAvailability>();
            if (availability == null)
            {
                return filtered;
            }

            foreach (var item in availability)
            {
                if (providerPreferenceService.ShouldShowAvailability(item))
                {
                    filtered.Add(item);
                }
            }

            return filtered;
        }

        private void RenderWatchProviders(
            IList<MediaAvailability> availability,
            bool hadRawAvailability,
            CancellationToken cancellationToken)
        {
            DisposeProviderControls();
            if (availability == null || availability.Count == 0)
            {
                lblAvailabilityStatus.Text = hadRawAvailability
                    ? "Nenhuma opção compatível com os serviços habilitados foi encontrada."
                    : "Nenhuma disponibilidade encontrada para o país configurado.";
                return;
            }

            lblAvailabilityStatus.Text = string.Empty;
            foreach (var item in availability)
            {
                if (item == null || item.Provider == null ||
                    string.IsNullOrWhiteSpace(item.Provider.Name))
                {
                    continue;
                }

                var row = new Panel
                {
                    Width = Math.Max(470, providerListPanel.ClientSize.Width - 25),
                    Height = 48,
                    Margin = new Padding(0, 0, 0, 4)
                };
                var logo = new PictureBox
                {
                    BackColor = Color.Gainsboro,
                    Location = new Point(0, 2),
                    Size = new Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                var name = new Label
                {
                    AutoEllipsis = true,
                    Location = new Point(50, 3),
                    Size = new Size(250, 21),
                    Text = item.Provider.Name
                };
                var type = new Label
                {
                    AutoEllipsis = true,
                    ForeColor = Color.DimGray,
                    Location = new Point(50, 24),
                    Size = new Size(250, 21),
                    Text = GetAvailabilityTypeLabel(item.AvailabilityType)
                };

                row.Controls.Add(logo);
                row.Controls.Add(name);
                row.Controls.Add(type);
                AddWatchLink(row, item.WatchUrl, 315);
                providerListPanel.Controls.Add(row);

                if (!string.IsNullOrWhiteSpace(item.Provider.LogoUrl))
                {
                    _ = LoadProviderLogoAsync(logo, item.Provider.LogoUrl, cancellationToken);
                }
            }

            if (providerListPanel.Controls.Count == 0)
            {
                lblAvailabilityStatus.Text = hadRawAvailability
                    ? "Nenhuma opção compatível com os serviços habilitados foi encontrada."
                    : "Nenhuma disponibilidade encontrada para o país configurado.";
            }
        }

        private async Task LoadProviderLogoAsync(
            PictureBox pictureBox,
            string logoUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                var bytes = await httpService.GetBytesAsync(logoUrl, cancellationToken);
                if (cancellationToken.IsCancellationRequested || IsDisposed || Disposing || pictureBox.IsDisposed)
                {
                    return;
                }

                using (var stream = new MemoryStream(bytes))
                using (var sourceImage = Image.FromStream(stream))
                {
                    var image = new Bitmap(sourceImage);
                    providerImages.Add(image);
                    pictureBox.Image = image;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                // A logo is optional; the provider name remains visible.
            }
        }

        private void AddWatchLink(Control parent, string watchUrl, int left)
        {
            Uri uri;
            if (!Uri.TryCreate(watchUrl, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return;
            }

            var link = new LinkLabel
            {
                AutoSize = true,
                LinkColor = Color.RoyalBlue,
                Location = new Point(left, 14),
                Text = "Ver opções"
            };
            link.Click += delegate { OpenWatchUrl(uri.AbsoluteUri); };
            parent.Controls.Add(link);
        }

        private static string GetAvailabilityTypeLabel(AvailabilityType availabilityType)
        {
            switch (availabilityType)
            {
                case AvailabilityType.Subscription:
                    return "Assinatura";
                case AvailabilityType.Free:
                    return "Grátis";
                case AvailabilityType.Ads:
                    return "Grátis com anúncios";
                case AvailabilityType.Rent:
                    return "Aluguel";
                case AvailabilityType.Buy:
                    return "Compra";
                default:
                    return availabilityType.ToString();
            }
        }

        private static void OpenWatchUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                // Browser launch failures must not interrupt the details screen.
            }
        }

        private void DisposeProviderControls()
        {
            DisposeProviderImages();
            while (providerListPanel.Controls.Count > 0)
            {
                providerListPanel.Controls[0].Dispose();
            }
        }

        private void DisposeProviderImages()
        {
            foreach (var image in providerImages)
            {
                image.Dispose();
            }

            providerImages.Clear();
        }

        private void CancelProvidersLoading()
        {
            if (providersCancellation == null)
            {
                return;
            }

            providersCancellation.Cancel();
            providersCancellation.Dispose();
            providersCancellation = null;
        }

        private void UpdateFavoriteButton()
        {
            btnFavorite.Text = favoritesService.IsFavorite(media)
                ? "Remover favorito"
                : "Favoritar";
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            try
            {
                favoritesService.Toggle(media);
                UpdateFavoriteButton();
            }
            catch (FavoritesPersistenceException)
            {
                MessageBox.Show(
                    this,
                    "N\u00e3o foi poss\u00edvel salvar os favoritos.",
                    "Favoritos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                UpdateFavoriteButton();
            }
        }

        private void StartPosterLoading(string posterUrl)
        {
            currentPosterUrl = posterUrl;
            if (string.IsNullOrWhiteSpace(posterUrl))
            {
                return;
            }

            posterCancellation = new CancellationTokenSource();
            _ = LoadPosterAsync(posterUrl, posterCancellation.Token);
        }

        private void UpdatePoster(string posterUrl)
        {
            if (string.Equals(currentPosterUrl, posterUrl, StringComparison.Ordinal))
            {
                return;
            }

            CancelPosterLoading();
            SetPlaceholder();
            StartPosterLoading(posterUrl);
        }

        private async Task LoadPosterAsync(string posterUrl, CancellationToken cancellationToken)
        {
            try
            {
                var bytes = await httpService.GetBytesAsync(posterUrl, cancellationToken);
                if (cancellationToken.IsCancellationRequested || IsDisposed || Disposing)
                {
                    return;
                }

                using (var stream = new MemoryStream(bytes))
                using (var sourceImage = Image.FromStream(stream))
                {
                    SetPosterImage(new Bitmap(sourceImage));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (HttpServiceException)
            {
                SetPlaceholderIfAvailable();
            }
            catch (Exception)
            {
                SetPlaceholderIfAvailable();
            }
        }

        private void SetPlaceholderIfAvailable()
        {
            if (!IsDisposed && !Disposing)
            {
                SetPlaceholder();
            }
        }

        private void SetPosterImage(Image image)
        {
            if (IsDisposed || Disposing)
            {
                image.Dispose();
                return;
            }

            var previousImage = picPoster.Image;
            picPoster.Image = image;
            lblPosterPlaceholder.Visible = false;
            if (previousImage != null)
            {
                previousImage.Dispose();
            }
        }

        private void SetPlaceholder()
        {
            var previousImage = picPoster.Image;
            picPoster.Image = null;
            lblPosterPlaceholder.Visible = true;
            if (previousImage != null)
            {
                previousImage.Dispose();
            }
        }

        private void CancelPosterLoading()
        {
            if (posterCancellation == null)
            {
                return;
            }

            posterCancellation.Cancel();
            posterCancellation.Dispose();
            posterCancellation = null;
        }

    }
}
