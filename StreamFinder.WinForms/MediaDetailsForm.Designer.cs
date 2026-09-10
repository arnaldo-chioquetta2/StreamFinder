namespace StreamFinder.WinForms
{
    partial class MediaDetailsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picPoster;
        private System.Windows.Forms.Label lblPosterPlaceholder;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblOriginalTitle;
        private System.Windows.Forms.Label lblOriginalTitleValue;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Label lblYearValue;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.Label lblRatingValue;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblTypeValue;
        private System.Windows.Forms.Label lblGenres;
        private System.Windows.Forms.Label lblGenresValue;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.TextBox txtOverview;
        private System.Windows.Forms.Label lblTrailerStatus;
        private System.Windows.Forms.Button btnWatchTrailer;
        private System.Windows.Forms.Label lblAvailability;
        private System.Windows.Forms.Label lblAvailabilityStatus;
        private System.Windows.Forms.FlowLayoutPanel providerListPanel;
        private System.Windows.Forms.Button btnFavorite;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelPosterLoading();
                CancelProvidersLoading();
                CancelTrailerLoading();
                if (picPoster != null && picPoster.Image != null)
                {
                    picPoster.Image.Dispose();
                    picPoster.Image = null;
                }
                DisposeProviderImages();
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            picPoster = new System.Windows.Forms.PictureBox();
            lblPosterPlaceholder = new System.Windows.Forms.Label();
            lblTitle = new System.Windows.Forms.Label();
            lblOriginalTitle = new System.Windows.Forms.Label();
            lblOriginalTitleValue = new System.Windows.Forms.Label();
            lblYear = new System.Windows.Forms.Label();
            lblYearValue = new System.Windows.Forms.Label();
            lblRating = new System.Windows.Forms.Label();
            lblRatingValue = new System.Windows.Forms.Label();
            lblType = new System.Windows.Forms.Label();
            lblTypeValue = new System.Windows.Forms.Label();
            lblGenres = new System.Windows.Forms.Label();
            lblGenresValue = new System.Windows.Forms.Label();
            lblOverview = new System.Windows.Forms.Label();
            txtOverview = new System.Windows.Forms.TextBox();
            lblTrailerStatus = new System.Windows.Forms.Label();
            btnWatchTrailer = new System.Windows.Forms.Button();
            lblAvailability = new System.Windows.Forms.Label();
            lblAvailabilityStatus = new System.Windows.Forms.Label();
            providerListPanel = new System.Windows.Forms.FlowLayoutPanel();
            btnFavorite = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(picPoster)).BeginInit();
            SuspendLayout();
            picPoster.BackColor = System.Drawing.Color.Gainsboro;
            picPoster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            picPoster.Location = new System.Drawing.Point(18, 18);
            picPoster.Size = new System.Drawing.Size(265, 390);
            picPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            lblPosterPlaceholder.AutoSize = false;
            lblPosterPlaceholder.BackColor = System.Drawing.Color.Gainsboro;
            lblPosterPlaceholder.ForeColor = System.Drawing.Color.DimGray;
            lblPosterPlaceholder.Location = new System.Drawing.Point(18, 18);
            lblPosterPlaceholder.Size = new System.Drawing.Size(265, 390);
            lblPosterPlaceholder.Text = "Sem imagem";
            lblPosterPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(305, 18);
            lblTitle.Size = new System.Drawing.Size(525, 58);
            lblOriginalTitle = CreateCaption("Título original:", 305, 91);
            lblOriginalTitleValue = CreateValue(415, 91, 415, 22);
            lblYear = CreateCaption("Ano:", 305, 126);
            lblYearValue = CreateValue(350, 126, 65, 22);
            lblRating = CreateCaption("Nota:", 430, 126);
            lblRatingValue = CreateValue(480, 126, 65, 22);
            lblType = CreateCaption("Tipo:", 560, 126);
            lblTypeValue = CreateValue(610, 126, 120, 22);
            lblGenres = CreateCaption("Gêneros:", 305, 162);
            lblGenresValue = CreateValue(305, 186, 525, 42);
            lblOverview = CreateCaption("Sinopse:", 305, 245);
            txtOverview.BackColor = System.Drawing.SystemColors.Window;
            txtOverview.Location = new System.Drawing.Point(305, 270);
            txtOverview.Multiline = true;
            txtOverview.ReadOnly = true;
            txtOverview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtOverview.Size = new System.Drawing.Size(525, 145);
            txtOverview.TabStop = false;
            lblTrailerStatus.AutoEllipsis = true;
            lblTrailerStatus.ForeColor = System.Drawing.Color.DimGray;
            lblTrailerStatus.Location = new System.Drawing.Point(305, 425);
            lblTrailerStatus.Size = new System.Drawing.Size(285, 30);
            lblTrailerStatus.Text = "Buscando trailer...";
            btnWatchTrailer.Enabled = false;
            btnWatchTrailer.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnWatchTrailer.Location = new System.Drawing.Point(600, 422);
            btnWatchTrailer.Size = new System.Drawing.Size(230, 34);
            btnWatchTrailer.Text = "Assistir trailer";
            btnWatchTrailer.UseVisualStyleBackColor = true;
            btnWatchTrailer.Click += new System.EventHandler(btnWatchTrailer_Click);
            lblAvailability = CreateCaption("Onde assistir", 305, 465);
            lblAvailabilityStatus.AutoEllipsis = true;
            lblAvailabilityStatus.ForeColor = System.Drawing.Color.DimGray;
            lblAvailabilityStatus.Location = new System.Drawing.Point(305, 489);
            lblAvailabilityStatus.Size = new System.Drawing.Size(525, 20);
            lblAvailabilityStatus.Text = "Buscando disponibilidade...";
            providerListPanel.AutoScroll = true;
            providerListPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            providerListPanel.Location = new System.Drawing.Point(305, 512);
            providerListPanel.Size = new System.Drawing.Size(525, 65);
            providerListPanel.WrapContents = false;
            btnFavorite.Location = new System.Drawing.Point(605, 615);
            btnFavorite.Size = new System.Drawing.Size(110, 32);
            btnFavorite.Text = "Favoritar";
            btnFavorite.UseVisualStyleBackColor = true;
            btnFavorite.Click += new System.EventHandler(btnFavorite_Click);
            btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnClose.Location = new System.Drawing.Point(720, 615);
            btnClose.Size = new System.Drawing.Size(110, 32);
            btnClose.Text = "Fechar";
            btnClose.UseVisualStyleBackColor = true;
            Controls.Add(btnClose);
            Controls.Add(btnFavorite);
            Controls.Add(btnWatchTrailer);
            Controls.Add(lblTrailerStatus);
            Controls.Add(providerListPanel);
            Controls.Add(lblAvailabilityStatus);
            Controls.Add(lblAvailability);
            Controls.Add(txtOverview);
            Controls.Add(lblOverview);
            Controls.Add(lblGenresValue);
            Controls.Add(lblGenres);
            Controls.Add(lblTypeValue);
            Controls.Add(lblType);
            Controls.Add(lblRatingValue);
            Controls.Add(lblRating);
            Controls.Add(lblYearValue);
            Controls.Add(lblYear);
            Controls.Add(lblOriginalTitleValue);
            Controls.Add(lblOriginalTitle);
            Controls.Add(lblTitle);
            Controls.Add(lblPosterPlaceholder);
            Controls.Add(picPoster);
            AcceptButton = btnClose;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(850, 665);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Detalhes";
            ((System.ComponentModel.ISupportInitialize)(picPoster)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private static System.Windows.Forms.Label CreateCaption(string text, int x, int y)
        {
            return new System.Windows.Forms.Label
            {
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(x, y),
                Text = text
            };
        }

        private static System.Windows.Forms.Label CreateValue(int x, int y, int width, int height)
        {
            return new System.Windows.Forms.Label
            {
                AutoEllipsis = true,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, height),
                Text = "-"
            };
        }
    }
}
