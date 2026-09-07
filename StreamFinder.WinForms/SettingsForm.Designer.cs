namespace StreamFinder.WinForms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupGeneral;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.TextBox txtCountry;
        private System.Windows.Forms.CheckBox chkEnableScraping;
        private System.Windows.Forms.Label lblCacheHours;
        private System.Windows.Forms.NumericUpDown nudCacheHours;
        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.GroupBox groupPaidProviders;
        private System.Windows.Forms.CheckBox chkNetflix;
        private System.Windows.Forms.CheckBox chkPrimeVideo;
        private System.Windows.Forms.CheckBox chkDisneyPlus;
        private System.Windows.Forms.CheckBox chkMax;
        private System.Windows.Forms.CheckBox chkAppleTV;
        private System.Windows.Forms.CheckBox chkParamountPlus;
        private System.Windows.Forms.CheckBox chkGloboplay;
        private System.Windows.Forms.GroupBox groupFreeProviders;
        private System.Windows.Forms.CheckBox chkYouTube;
        private System.Windows.Forms.CheckBox chkPlutoTV;
        private System.Windows.Forms.CheckBox chkMercadoPlay;
        private System.Windows.Forms.GroupBox groupApis;
        private System.Windows.Forms.Label lblTmdbApiKey;
        private System.Windows.Forms.TextBox txtTmdbApiKey;
        private System.Windows.Forms.Label lblYouTubeApiKey;
        private System.Windows.Forms.TextBox txtYouTubeApiKey;
        private System.Windows.Forms.CheckBox chkShowApiKeys;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupGeneral = new System.Windows.Forms.GroupBox();
            lblCountry = new System.Windows.Forms.Label();
            txtCountry = new System.Windows.Forms.TextBox();
            chkEnableScraping = new System.Windows.Forms.CheckBox();
            lblCacheHours = new System.Windows.Forms.Label();
            nudCacheHours = new System.Windows.Forms.NumericUpDown();
            btnClearCache = new System.Windows.Forms.Button();
            groupPaidProviders = new System.Windows.Forms.GroupBox();
            chkNetflix = new System.Windows.Forms.CheckBox();
            chkPrimeVideo = new System.Windows.Forms.CheckBox();
            chkDisneyPlus = new System.Windows.Forms.CheckBox();
            chkMax = new System.Windows.Forms.CheckBox();
            chkAppleTV = new System.Windows.Forms.CheckBox();
            chkParamountPlus = new System.Windows.Forms.CheckBox();
            chkGloboplay = new System.Windows.Forms.CheckBox();
            groupFreeProviders = new System.Windows.Forms.GroupBox();
            chkYouTube = new System.Windows.Forms.CheckBox();
            chkPlutoTV = new System.Windows.Forms.CheckBox();
            chkMercadoPlay = new System.Windows.Forms.CheckBox();
            groupApis = new System.Windows.Forms.GroupBox();
            lblTmdbApiKey = new System.Windows.Forms.Label();
            txtTmdbApiKey = new System.Windows.Forms.TextBox();
            lblYouTubeApiKey = new System.Windows.Forms.Label();
            txtYouTubeApiKey = new System.Windows.Forms.TextBox();
            chkShowApiKeys = new System.Windows.Forms.CheckBox();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            groupGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(nudCacheHours)).BeginInit();
            groupPaidProviders.SuspendLayout();
            groupFreeProviders.SuspendLayout();
            groupApis.SuspendLayout();
            SuspendLayout();
            // groupGeneral
            groupGeneral.Controls.Add(lblCountry);
            groupGeneral.Controls.Add(txtCountry);
            groupGeneral.Controls.Add(chkEnableScraping);
            groupGeneral.Controls.Add(lblCacheHours);
            groupGeneral.Controls.Add(nudCacheHours);
            groupGeneral.Controls.Add(btnClearCache);
            groupGeneral.Location = new System.Drawing.Point(12, 12);
            groupGeneral.Size = new System.Drawing.Size(660, 105);
            groupGeneral.Text = "Configurações gerais";
            lblCountry.AutoSize = true;
            lblCountry.Location = new System.Drawing.Point(18, 28);
            lblCountry.Text = "País";
            txtCountry.Location = new System.Drawing.Point(75, 25);
            txtCountry.Size = new System.Drawing.Size(70, 23);
            chkEnableScraping.AutoSize = true;
            chkEnableScraping.Location = new System.Drawing.Point(180, 28);
            chkEnableScraping.Text = "Ativar scraping quando necessário";
            lblCacheHours.AutoSize = true;
            lblCacheHours.Location = new System.Drawing.Point(18, 68);
            lblCacheHours.Text = "Horas de cache";
            nudCacheHours.Location = new System.Drawing.Point(105, 65);
            nudCacheHours.Minimum = 1;
            nudCacheHours.Maximum = 168;
            nudCacheHours.Value = 12;
            // btnClearCache
            btnClearCache.Location = new System.Drawing.Point(400, 62);
            btnClearCache.Size = new System.Drawing.Size(140, 28);
            btnClearCache.Text = "Limpar Cache";
            btnClearCache.UseVisualStyleBackColor = true;
            btnClearCache.Click += new System.EventHandler(btnClearCache_Click);
            // groupPaidProviders
            groupPaidProviders.Controls.Add(chkNetflix);
            groupPaidProviders.Controls.Add(chkPrimeVideo);
            groupPaidProviders.Controls.Add(chkDisneyPlus);
            groupPaidProviders.Controls.Add(chkMax);
            groupPaidProviders.Controls.Add(chkAppleTV);
            groupPaidProviders.Controls.Add(chkParamountPlus);
            groupPaidProviders.Controls.Add(chkGloboplay);
            groupPaidProviders.Location = new System.Drawing.Point(12, 130);
            groupPaidProviders.Size = new System.Drawing.Size(320, 245);
            groupPaidProviders.Text = "Serviços pagos";
            ConfigureCheckBox(chkNetflix, "Netflix", 18, 28);
            ConfigureCheckBox(chkPrimeVideo, "Amazon Prime Video", 18, 58);
            ConfigureCheckBox(chkDisneyPlus, "Disney+", 18, 88);
            ConfigureCheckBox(chkMax, "Max", 18, 118);
            ConfigureCheckBox(chkAppleTV, "Apple TV+", 18, 148);
            ConfigureCheckBox(chkParamountPlus, "Paramount+", 165, 28);
            ConfigureCheckBox(chkGloboplay, "Globoplay", 165, 58);
            // groupFreeProviders
            groupFreeProviders.Controls.Add(chkYouTube);
            groupFreeProviders.Controls.Add(chkPlutoTV);
            groupFreeProviders.Controls.Add(chkMercadoPlay);
            groupFreeProviders.Location = new System.Drawing.Point(350, 130);
            groupFreeProviders.Size = new System.Drawing.Size(322, 145);
            groupFreeProviders.Text = "Serviços gratuitos";
            ConfigureCheckBox(chkYouTube, "YouTube", 18, 28);
            ConfigureCheckBox(chkPlutoTV, "Pluto TV", 18, 58);
            ConfigureCheckBox(chkMercadoPlay, "Mercado Play", 18, 88);
            // groupApis
            groupApis.Controls.Add(lblTmdbApiKey);
            groupApis.Controls.Add(txtTmdbApiKey);
            groupApis.Controls.Add(lblYouTubeApiKey);
            groupApis.Controls.Add(txtYouTubeApiKey);
            groupApis.Controls.Add(chkShowApiKeys);
            groupApis.Location = new System.Drawing.Point(12, 390);
            groupApis.Size = new System.Drawing.Size(660, 145);
            groupApis.Text = "APIs";
            lblTmdbApiKey.AutoSize = true;
            lblTmdbApiKey.Location = new System.Drawing.Point(18, 30);
            lblTmdbApiKey.Text = "TMDb API Key";
            txtTmdbApiKey.Location = new System.Drawing.Point(145, 27);
            txtTmdbApiKey.Size = new System.Drawing.Size(480, 23);
            txtTmdbApiKey.UseSystemPasswordChar = true;
            lblYouTubeApiKey.AutoSize = true;
            lblYouTubeApiKey.Location = new System.Drawing.Point(18, 68);
            lblYouTubeApiKey.Text = "YouTube API Key";
            txtYouTubeApiKey.Location = new System.Drawing.Point(145, 65);
            txtYouTubeApiKey.Size = new System.Drawing.Size(480, 23);
            txtYouTubeApiKey.UseSystemPasswordChar = true;
            chkShowApiKeys.AutoSize = true;
            chkShowApiKeys.Location = new System.Drawing.Point(145, 105);
            chkShowApiKeys.Text = "Mostrar chaves";
            chkShowApiKeys.CheckedChanged += new System.EventHandler(chkShowApiKeys_CheckedChanged);
            // buttons
            btnSave.Location = new System.Drawing.Point(486, 560);
            btnSave.Size = new System.Drawing.Size(88, 30);
            btnSave.Text = "Salvar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += new System.EventHandler(btnSave_Click);
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(584, 560);
            btnCancel.Size = new System.Drawing.Size(88, 30);
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += new System.EventHandler(btnCancel_Click);
            // form
            AcceptButton = btnSave;
            CancelButton = btnCancel;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(700, 615);
            Controls.Add(groupApis);
            Controls.Add(groupFreeProviders);
            Controls.Add(groupPaidProviders);
            Controls.Add(groupGeneral);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Configurações";
            groupGeneral.ResumeLayout(false);
            groupGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(nudCacheHours)).EndInit();
            groupPaidProviders.ResumeLayout(false);
            groupFreeProviders.ResumeLayout(false);
            groupApis.ResumeLayout(false);
            groupApis.PerformLayout();
            ResumeLayout(false);
        }

        private static void ConfigureCheckBox(System.Windows.Forms.CheckBox checkBox, string text, int left, int top)
        {
            checkBox.AutoSize = true;
            checkBox.Location = new System.Drawing.Point(left, top);
            checkBox.Text = text;
        }
    }
}
