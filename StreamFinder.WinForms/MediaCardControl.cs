using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamFinder.WinForms.Models;
using StreamFinder.WinForms.Services;

namespace StreamFinder.WinForms
{
    public partial class MediaCardControl : UserControl
    {
        private readonly HttpService httpService;
        private CancellationTokenSource posterCancellation;
        private MediaItem media;

        public MediaCardControl()
        {
            InitializeComponent();
            httpService = new HttpService();
            SetPlaceholder();
        }

        public event EventHandler DetailsClicked;
        public event EventHandler FavoriteClicked;

        public MediaItem Media
        {
            get { return media; }
        }

        public void SetFavoriteState(bool isFavorite)
        {
            btnFavorite.Text = isFavorite ? "Remover favorito" : "Favoritar";
        }

        public void SetMedia(MediaItem value)
        {
            media = value;
            CancelPosterLoading();
            SetPlaceholder();

            if (media == null)
            {
                lblTitle.Text = "Sem título";
                lblYear.Text = "Ano: *";
                lblRating.Text = "Nota: *";
                lblType.Text = "Tipo: -";
                return;
            }

            lblTitle.Text = string.IsNullOrWhiteSpace(media.Title) ? "Sem título" : media.Title;
            lblYear.Text = string.Format("Ano: {0}", media.Year.HasValue ? media.Year.Value.ToString() : "*");
            lblRating.Text = string.Format(
                "Nota: {0}",
                media.Rating.HasValue
                    ? media.Rating.Value.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))
                    : "*");
            lblType.Text = string.Format("Tipo: {0}", media.MediaType == MediaType.Movie ? "Filme" : "Série");

            if (!string.IsNullOrWhiteSpace(media.PosterUrl))
            {
                posterCancellation = new CancellationTokenSource();
                _ = LoadPosterAsync(media.PosterUrl, posterCancellation.Token);
            }
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
                    var independentImage = new Bitmap(sourceImage);
                    SetPosterImage(independentImage);
                }
            }
            catch (OperationCanceledException)
            {
                // A new card value or disposal cancelled the previous image request.
            }
            catch (Exception)
            {
                if (!IsDisposed && !Disposing)
                {
                    SetPlaceholder();
                }
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
            if (previousImage != null)
            {
                previousImage.Dispose();
            }
        }

        private void SetPlaceholder()
        {
            var placeholder = new Bitmap(picPoster.Width, picPoster.Height);
            using (var graphics = Graphics.FromImage(placeholder))
            using (var brush = new SolidBrush(Color.Gainsboro))
            using (var textBrush = new SolidBrush(Color.DimGray))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.FillRectangle(brush, 0, 0, placeholder.Width, placeholder.Height);
                graphics.DrawString("Sem imagem", picPoster.Font, textBrush,
                    new RectangleF(0, 0, placeholder.Width, placeholder.Height), format);
            }

            SetPosterImage(placeholder);
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

        private void btnDetails_Click(object sender, EventArgs e)
        {
            var handler = DetailsClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            var handler = FavoriteClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelPosterLoading();
                if (picPoster != null && picPoster.Image != null)
                {
                    picPoster.Image.Dispose();
                    picPoster.Image = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
