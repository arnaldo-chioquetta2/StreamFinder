namespace StreamFinder.WinForms
{
    partial class MediaCardControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picPoster;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Button btnFavorite;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            picPoster = new System.Windows.Forms.PictureBox();
            lblTitle = new System.Windows.Forms.Label();
            lblYear = new System.Windows.Forms.Label();
            lblRating = new System.Windows.Forms.Label();
            lblType = new System.Windows.Forms.Label();
            btnDetails = new System.Windows.Forms.Button();
            btnFavorite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(picPoster)).BeginInit();
            SuspendLayout();
            // picPoster
            picPoster.BackColor = System.Drawing.Color.Gainsboro;
            picPoster.Location = new System.Drawing.Point(10, 10);
            picPoster.Size = new System.Drawing.Size(200, 185);
            picPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // lblTitle
            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(10, 202);
            lblTitle.Size = new System.Drawing.Size(200, 34);
            lblTitle.Text = "Sem título";
            // lblYear
            lblYear.AutoSize = true;
            lblYear.Location = new System.Drawing.Point(10, 241);
            lblYear.Text = "Ano: *";
            // lblRating
            lblRating.AutoSize = true;
            lblRating.Location = new System.Drawing.Point(78, 241);
            lblRating.Text = "Nota: *";
            // lblType
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(10, 263);
            lblType.Text = "Tipo: -";
            // btnDetails
            btnDetails.Location = new System.Drawing.Point(10, 294);
            btnDetails.Size = new System.Drawing.Size(94, 30);
            btnDetails.Text = "Detalhes";
            btnDetails.UseVisualStyleBackColor = true;
            btnDetails.Click += new System.EventHandler(btnDetails_Click);
            // btnFavorite
            btnFavorite.Location = new System.Drawing.Point(116, 294);
            btnFavorite.Size = new System.Drawing.Size(94, 30);
            btnFavorite.Text = "Favoritar";
            btnFavorite.UseVisualStyleBackColor = true;
            btnFavorite.Click += new System.EventHandler(btnFavorite_Click);
            // MediaCardControl
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(btnFavorite);
            Controls.Add(btnDetails);
            Controls.Add(lblType);
            Controls.Add(lblRating);
            Controls.Add(lblYear);
            Controls.Add(lblTitle);
            Controls.Add(picPoster);
            Margin = new System.Windows.Forms.Padding(8);
            Name = "MediaCardControl";
            Size = new System.Drawing.Size(220, 360);
            ((System.ComponentModel.ISupportInitialize)(picPoster)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
