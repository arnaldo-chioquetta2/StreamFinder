using System;
using System.Windows.Forms;
using StreamFinder.WinForms.Models;
using StreamFinder.WinForms.Services;

namespace StreamFinder.WinForms
{
    public partial class SettingsForm : Form
    {
        private readonly IniFileService iniFileService;
        private readonly CacheService cacheService;

        public SettingsForm()
        {
            InitializeComponent();
            iniFileService = new IniFileService();
            cacheService = new CacheService();
            LoadSettingsIntoControls();
        }

        private void LoadSettingsIntoControls()
        {
            var settings = iniFileService.LoadSettings();

            txtCountry.Text = settings.Country;
            chkEnableScraping.Checked = settings.EnableScraping;
            nudCacheHours.Value = Math.Max(nudCacheHours.Minimum, Math.Min(nudCacheHours.Maximum, settings.CacheHours));
            txtTmdbApiKey.Text = settings.TmdbApiKey;
            txtYouTubeApiKey.Text = settings.YouTubeApiKey;

            chkNetflix.Checked = iniFileService.IsProviderEnabled("Netflix");
            chkPrimeVideo.Checked = iniFileService.IsProviderEnabled("PrimeVideo");
            chkDisneyPlus.Checked = iniFileService.IsProviderEnabled("DisneyPlus");
            chkMax.Checked = iniFileService.IsProviderEnabled("Max");
            chkAppleTV.Checked = iniFileService.IsProviderEnabled("AppleTV");
            chkParamountPlus.Checked = iniFileService.IsProviderEnabled("ParamountPlus");
            chkGloboplay.Checked = iniFileService.IsProviderEnabled("Globoplay");
            chkYouTube.Checked = iniFileService.IsProviderEnabled("YouTube");
            chkPlutoTV.Checked = iniFileService.IsProviderEnabled("PlutoTV");
            chkMercadoPlay.Checked = iniFileService.IsProviderEnabled("MercadoPlay");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var country = txtCountry.Text.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(country))
            {
                MessageBox.Show(this, "Informe o código do país.", "Configurações", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCountry.Focus();
                return;
            }

            iniFileService.SaveSettings(new AppSettings
            {
                Country = country,
                EnableScraping = chkEnableScraping.Checked,
                CacheHours = Decimal.ToInt32(nudCacheHours.Value),
                TmdbApiKey = txtTmdbApiKey.Text,
                YouTubeApiKey = txtYouTubeApiKey.Text
            });

            iniFileService.SetProviderEnabled("Netflix", chkNetflix.Checked);
            iniFileService.SetProviderEnabled("PrimeVideo", chkPrimeVideo.Checked);
            iniFileService.SetProviderEnabled("DisneyPlus", chkDisneyPlus.Checked);
            iniFileService.SetProviderEnabled("Max", chkMax.Checked);
            iniFileService.SetProviderEnabled("AppleTV", chkAppleTV.Checked);
            iniFileService.SetProviderEnabled("ParamountPlus", chkParamountPlus.Checked);
            iniFileService.SetProviderEnabled("Globoplay", chkGloboplay.Checked);
            iniFileService.SetProviderEnabled("YouTube", chkYouTube.Checked);
            iniFileService.SetProviderEnabled("PlutoTV", chkPlutoTV.Checked);
            iniFileService.SetProviderEnabled("MercadoPlay", chkMercadoPlay.Checked);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "Deseja realmente limpar o cache local?",
                    "Cache",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                cacheService.Clear();
                MessageBox.Show(this, "Cache limpo com sucesso.", "Cache", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (CachePersistenceException)
            {
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel limpar o cache.", "Cache", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel limpar o cache.", "Cache", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkShowApiKeys_CheckedChanged(object sender, EventArgs e)
        {
            txtTmdbApiKey.UseSystemPasswordChar = !chkShowApiKeys.Checked;
            txtYouTubeApiKey.UseSystemPasswordChar = !chkShowApiKeys.Checked;
        }
    }
}
