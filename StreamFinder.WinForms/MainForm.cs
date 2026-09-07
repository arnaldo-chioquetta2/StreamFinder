using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamFinder.WinForms.Models;
using StreamFinder.WinForms.Services;

namespace StreamFinder.WinForms
{
    public partial class MainForm : Form
    {
        private enum MainViewMode
        {
            Search,
            Favorites,
            History
        }

        private readonly TmdbService tmdbService;
        private readonly FavoritesService favoritesService;
        private readonly SearchHistoryService searchHistoryService;
        private MainViewMode currentView;

        public MainForm()
        {
            InitializeComponent();
            tmdbService = new TmdbService();
            favoritesService = new FavoritesService();
            searchHistoryService = new SearchHistoryService();
            currentView = MainViewMode.Search;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            await ExecuteSearchAsync();
        }

        private async Task ExecuteSearchAsync()
        {
            var query = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                toolStripStatusLabelMessage.Text = "Digite algo para pesquisar.";
                return;
            }

            currentView = MainViewMode.Search;
            btnSearch.Enabled = false;
            toolStripStatusLabelMessage.Text = "Pesquisando...";

            try
            {
                var results = await tmdbService.SearchAsync(query);
                var historySaveFailed = false;

                try
                {
                    searchHistoryService.Add(query, GetSelectedSearchType());
                }
                catch (SearchHistoryPersistenceException)
                {
                    historySaveFailed = true;
                }

                var filteredResults = FilterByMediaType(results);
                ClearResults();
                foreach (var item in filteredResults)
                {
                    flowResults.Controls.Add(CreateMediaCard(item));
                }

                if (historySaveFailed)
                {
                    toolStripStatusLabelMessage.Text = "N\u00e3o foi poss\u00edvel salvar o hist\u00f3rico.";
                }
                else if (filteredResults.Count == 0)
                {
                    toolStripStatusLabelMessage.Text = "Nenhum resultado encontrado.";
                }
                else
                {
                    toolStripStatusLabelMessage.Text = string.Format(
                        "{0} resultado(s) encontrado(s).",
                        filteredResults.Count);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Configure sua chave do TMDb em Configura\u00e7\u00f5es.")
                {
                    toolStripStatusLabelMessage.Text = "Pesquisa n\u00e3o realizada.";
                    MessageBox.Show(this, ex.Message, "Pesquisa TMDb", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    toolStripStatusLabelMessage.Text = "Erro ao pesquisar.";
                    MessageBox.Show(this, "N\u00e3o foi poss\u00edvel realizar a pesquisa neste momento.", "Pesquisa TMDb", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                toolStripStatusLabelMessage.Text = "Erro ao pesquisar.";
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel realizar a pesquisa neste momento.", "Pesquisa TMDb", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnSearch.Enabled = true;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    toolStripStatusLabelMessage.Text = "Configura\u00e7\u00f5es salvas.";
                }
            }
        }

        private List<MediaItem> FilterByMediaType(List<MediaItem> results)
        {
            var filteredResults = new List<MediaItem>();
            if (results == null)
            {
                return filteredResults;
            }

            var selectedIndex = cmbMediaType.SelectedIndex;
            foreach (var item in results)
            {
                if (item == null || (selectedIndex == 1 && item.MediaType != MediaType.Movie) ||
                    (selectedIndex == 2 && item.MediaType != MediaType.Tv))
                {
                    continue;
                }

                filteredResults.Add(item);
            }

            return filteredResults;
        }

        private void ClearResults()
        {
            while (flowResults.Controls.Count > 0)
            {
                var control = flowResults.Controls[0];
                flowResults.Controls.RemoveAt(0);
                control.Dispose();
            }
        }

        private void mediaCard_DetailsClicked(object sender, EventArgs e)
        {
            var card = sender as MediaCardControl;
            var title = card == null || card.Media == null || string.IsNullOrWhiteSpace(card.Media.Title)
                ? "Sem t\u00edtulo"
                : card.Media.Title;
            toolStripStatusLabelMessage.Text = string.Format("Detalhes de: {0}", title);
        }

        private void mediaCard_FavoriteClicked(object sender, EventArgs e)
        {
            var card = sender as MediaCardControl;
            if (card == null || card.Media == null)
            {
                return;
            }

            try
            {
                var isFavorite = favoritesService.Toggle(card.Media);
                card.SetFavoriteState(isFavorite);

                if (isFavorite)
                {
                    toolStripStatusLabelMessage.Text = "Adicionado aos favoritos.";
                }
                else
                {
                    toolStripStatusLabelMessage.Text = "Removido dos favoritos.";
                    if (currentView == MainViewMode.Favorites)
                    {
                        flowResults.Controls.Remove(card);
                        card.Dispose();
                    }
                }
            }
            catch (FavoritesPersistenceException)
            {
                toolStripStatusLabelMessage.Text = "Erro ao salvar favorito.";
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel salvar os favoritos.", "Favoritos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                toolStripStatusLabelMessage.Text = "Erro ao salvar favorito.";
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel salvar os favoritos.", "Favoritos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnFavorites_Click(object sender, EventArgs e)
        {
            currentView = MainViewMode.Favorites;
            ClearResults();

            var favorites = favoritesService.GetAll();
            foreach (var favorite in favorites)
            {
                flowResults.Controls.Add(CreateMediaCard(favorite));
            }

            toolStripStatusLabelMessage.Text = favorites.Count == 0
                ? "Nenhum favorito salvo."
                : string.Format("{0} favorito(s).", favorites.Count);
        }

        private void btnSearchNavigation_Click(object sender, EventArgs e)
        {
            currentView = MainViewMode.Search;
            ClearResults();
            txtSearch.Focus();
            toolStripStatusLabelMessage.Text = "Digite algo para pesquisar.";
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            LoadHistoryView();
        }

        private void LoadHistoryView()
        {
            currentView = MainViewMode.History;
            ClearResults();

            var history = searchHistoryService.GetAll();
            flowResults.Controls.Add(CreateClearHistoryButton());
            foreach (var item in history)
            {
                flowResults.Controls.Add(CreateHistoryPanel(item));
            }

            toolStripStatusLabelMessage.Text = history.Count == 0
                ? "Nenhuma pesquisa no hist\u00f3rico."
                : string.Format("{0} pesquisa(s) no hist\u00f3rico.", history.Count);
        }

        private Button CreateClearHistoryButton()
        {
            var button = new Button
            {
                AutoSize = false,
                Height = 32,
                Margin = new Padding(8),
                Text = "Limpar hist\u00f3rico",
                Width = 500
            };
            button.Click += btnClearHistory_Click;
            return button;
        }

        private Panel CreateHistoryPanel(SearchHistoryItem item)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Height = 80,
                Margin = new Padding(8),
                Padding = new Padding(10),
                Tag = item,
                Width = 500
            };

            var queryLabel = new Label
            {
                AutoEllipsis = true,
                AutoSize = false,
                Location = new Point(10, 8),
                Size = new Size(460, 22),
                Text = string.Format("Pesquisa: {0}", string.IsNullOrWhiteSpace(item.Query) ? "-" : item.Query)
            };
            var typeLabel = new Label
            {
                AutoSize = true,
                Location = new Point(10, 35),
                Text = string.Format("Tipo: {0}", item.SearchType ?? string.Empty)
            };
            var dateLabel = new Label
            {
                AutoSize = true,
                Location = new Point(180, 35),
                Text = string.Format("Data: {0:dd/MM/yyyy HH:mm}", item.SearchedAt)
            };

            panel.Controls.Add(queryLabel);
            panel.Controls.Add(typeLabel);
            panel.Controls.Add(dateLabel);
            AttachHistoryDoubleClick(panel);
            return panel;
        }

        private static void AttachHistoryDoubleClick(Control control)
        {
            control.DoubleClick += HistoryItem_DoubleClick;
            foreach (Control child in control.Controls)
            {
                AttachHistoryDoubleClick(child);
            }
        }

        private static async void HistoryItem_DoubleClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            while (control != null && !(control.Tag is SearchHistoryItem))
            {
                control = control.Parent;
            }

            var item = control == null ? null : control.Tag as SearchHistoryItem;
            var form = control == null ? null : control.FindForm() as MainForm;
            if (item == null || form == null)
            {
                return;
            }

            form.txtSearch.Text = item.Query;
            form.cmbMediaType.SelectedIndex = form.GetSearchTypeIndex(item.SearchType);
            form.currentView = MainViewMode.Search;
            form.ClearResults();
            await form.ExecuteSearchAsync();
        }

        private int GetSearchTypeIndex(string searchType)
        {
            if (string.Equals(searchType, "Filmes", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            if (string.Equals(searchType, "S\u00e9ries", StringComparison.OrdinalIgnoreCase))
            {
                return 2;
            }

            return 0;
        }

        private string GetSelectedSearchType()
        {
            switch (cmbMediaType.SelectedIndex)
            {
                case 1:
                    return "Filmes";
                case 2:
                    return "S\u00e9ries";
                default:
                    return "Todos";
            }
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "Deseja realmente limpar todo o hist\u00f3rico de pesquisas?",
                    "Hist\u00f3rico",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                searchHistoryService.Clear();
                LoadHistoryView();
            }
            catch (SearchHistoryPersistenceException)
            {
                toolStripStatusLabelMessage.Text = "N\u00e3o foi poss\u00edvel acessar o hist\u00f3rico.";
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel acessar o hist\u00f3rico.", "Hist\u00f3rico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                toolStripStatusLabelMessage.Text = "N\u00e3o foi poss\u00edvel acessar o hist\u00f3rico.";
                MessageBox.Show(this, "N\u00e3o foi poss\u00edvel acessar o hist\u00f3rico.", "Hist\u00f3rico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private MediaCardControl CreateMediaCard(MediaItem item)
        {
            var card = new MediaCardControl();
            card.SetMedia(item);
            card.SetFavoriteState(favoritesService.IsFavorite(item));
            card.DetailsClicked += mediaCard_DetailsClicked;
            card.FavoriteClicked += mediaCard_FavoriteClicked;
            return card;
        }
    }
}
