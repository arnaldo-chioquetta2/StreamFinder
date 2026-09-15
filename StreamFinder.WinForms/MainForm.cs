using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            History,
            Discovery
        }

        private readonly TmdbService tmdbService;
        private readonly HttpService httpService;
        private readonly IniFileService iniFileService;
        private readonly AvailabilityFallbackService availabilityFallbackService;
        private readonly FavoritesService favoritesService;
        private readonly SearchHistoryService searchHistoryService;
        private readonly ProviderPreferenceService providerPreferenceService;
        private List<MediaItem> lastSearchResults = new List<MediaItem>();
        private List<MediaItem> lastOwnedFilterResults;
        private string lastOwnedFilterCandidateKey;
        private CancellationTokenSource filterCancellation;
        private int filterVersion;
        private string currentSearchQuery;
        private int currentSearchPage;
        private int totalSearchPages;
        private int totalSearchResults;
        private int searchOperationVersion;
        private bool isLoadingMore;
        private MainViewMode currentView;
        private CancellationTokenSource discoveryCancellation;
        private int discoveryOperationVersion;
        private List<MediaItem> discoveryAccumulatedItems = new List<MediaItem>();
        private int discoveryCurrentPage;
        private int discoveryTotalPages;
        private int discoveryTotalResults;
        private List<DiscoveryGenreOption> discoveryGenres;
        private bool isLoadingDiscovery;
        private bool isUpdatingDiscoveryControls;
        private DateTime? discoveryReleaseDateFrom;
        private DateTime? discoveryReleaseDateTo;

        public MainForm()
        {
            InitializeComponent();
            FormClosing += MainForm_FormClosing;
            httpService = new HttpService();
            iniFileService = new IniFileService();
            tmdbService = new TmdbService(httpService, iniFileService, new CacheService());
            var plutoSource = new PlutoTvFallbackSource(httpService, iniFileService);
            availabilityFallbackService = new AvailabilityFallbackService(
                iniFileService,
                new IAvailabilityFallbackSource[] { plutoSource });
            favoritesService = new FavoritesService();
            searchHistoryService = new SearchHistoryService();
            providerPreferenceService = new ProviderPreferenceService(iniFileService);
            currentView = MainViewMode.Search;
            UpdateLoadMoreButton();
        }

        private async void btnDiscoverNavigation_Click(object sender, EventArgs e)
        {
            await EnterDiscoveryModeAsync();
        }

        private async Task EnterDiscoveryModeAsync()
        {
            searchOperationVersion++;
            CancelFilterOperation();
            InvalidateDiscoveryOperation();
            currentView = MainViewMode.Discovery;
            ConfigureDiscoveryControls();
            ClearResults();
            discoveryAccumulatedItems = new List<MediaItem>();
            discoveryCurrentPage = 0;
            discoveryTotalPages = 0;
            discoveryTotalResults = 0;
            discoveryReleaseDateTo = DateTime.Today;
            discoveryReleaseDateFrom = discoveryReleaseDateTo.Value.AddMonths(-12);
            UpdateLoadMoreButton();

            if (discoveryGenres == null)
            {
                var version = discoveryOperationVersion;
                discoveryCancellation = new CancellationTokenSource();
                var token = discoveryCancellation.Token;
                btnDiscover.Enabled = false;
                toolStripStatusLabelMessage.Text = "Carregando gêneros...";
                try
                {
                    var genres = await tmdbService.GetDiscoveryGenresAsync(token);
                    if (version != discoveryOperationVersion || currentView != MainViewMode.Discovery)
                    {
                        return;
                    }

                    discoveryGenres = genres ?? new List<DiscoveryGenreOption>();
                    PopulateDiscoveryGenreOptions();
                    toolStripStatusLabelMessage.Text = "Selecione os filtros e clique em Descobrir.";
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception)
                {
                    if (version == discoveryOperationVersion && currentView == MainViewMode.Discovery)
                    {
                        discoveryGenres = new List<DiscoveryGenreOption>();
                        PopulateDiscoveryGenreOptions();
                        toolStripStatusLabelMessage.Text = "Não foi possível carregar os gêneros.";
                    }
                }
                finally
                {
                    if (version == discoveryOperationVersion && !IsDisposed && !Disposing)
                    {
                        discoveryCancellation.Dispose();
                        discoveryCancellation = null;
                        btnDiscover.Enabled = true;
                    }
                }
            }
            else
            {
                PopulateDiscoveryGenreOptions();
                toolStripStatusLabelMessage.Text = "Selecione os filtros e clique em Descobrir.";
            }
        }

        private void ConfigureDiscoveryControls()
        {
            isUpdatingDiscoveryControls = true;
            try
            {
                txtSearch.Enabled = false;
                btnSearch.Enabled = false;
                chkOwnedOnly.Enabled = false;
                btnDiscover.Visible = true;
                lblDiscoveryLanguage.Visible = true;
                cmbDiscoveryLanguage.Visible = true;
                btnDiscover.Enabled = true;
                SetDiscoverySortOptions();
                PopulateDiscoveryGenreOptions();
            }
            finally
            {
                isUpdatingDiscoveryControls = false;
            }
        }

        private void ConfigureSearchControls()
        {
            isUpdatingDiscoveryControls = true;
            try
            {
                txtSearch.Enabled = true;
                btnSearch.Enabled = true;
                chkOwnedOnly.Enabled = true;
                btnDiscover.Visible = false;
                lblDiscoveryLanguage.Visible = false;
                cmbDiscoveryLanguage.Visible = false;
                SetSearchSortOptions();
                UpdateGenreOptions(lastSearchResults);
            }
            finally
            {
                isUpdatingDiscoveryControls = false;
            }
        }

        private void SetDiscoverySortOptions()
        {
            var selected = cmbSort.SelectedItem == null ? null : cmbSort.SelectedItem.ToString();
            cmbSort.Items.Clear();
            cmbSort.Items.Add("Lançamentos");
            cmbSort.Items.Add("Melhor avaliados");
            cmbSort.Items.Add("Título");
            cmbSort.SelectedIndex = selected == "Melhor avaliados" ? 1 : selected == "Título" ? 2 : 0;
        }

        private void SetSearchSortOptions()
        {
            cmbSort.Items.Clear();
            cmbSort.Items.Add("Título");
            cmbSort.Items.Add("Ano");
            cmbSort.Items.Add("Nota");
            cmbSort.SelectedIndex = 0;
        }

        private void PopulateDiscoveryGenreOptions()
        {
            var selectedName = cmbGenre.SelectedItem == null ? null : cmbGenre.SelectedItem.ToString();
            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("Todos os gêneros");
            if (discoveryGenres != null)
            {
                foreach (var genre in discoveryGenres.OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    if (genre != null && !string.IsNullOrWhiteSpace(genre.Name))
                    {
                        cmbGenre.Items.Add(genre);
                    }
                }
            }

            var selectedIndex = 0;
            for (var i = 1; i < cmbGenre.Items.Count; i++)
            {
                if (string.Equals(cmbGenre.Items[i].ToString(), selectedName, StringComparison.CurrentCultureIgnoreCase))
                {
                    selectedIndex = i;
                    break;
                }
            }
            cmbGenre.SelectedIndex = selectedIndex;
        }

        private DiscoveryGenreOption GetSelectedDiscoveryGenre()
        {
            return cmbGenre.SelectedIndex > 0 ? cmbGenre.SelectedItem as DiscoveryGenreOption : null;
        }

        private DiscoverySortOption GetSelectedDiscoverySort()
        {
            switch (cmbSort.SelectedIndex)
            {
                case 1: return DiscoverySortOption.VoteAverage;
                case 2: return DiscoverySortOption.Title;
                default: return DiscoverySortOption.ReleaseDate;
            }
        }

        private DiscoveryLanguageOption GetSelectedDiscoveryLanguage()
        {
            if (cmbDiscoveryLanguage.SelectedIndex == 1) return DiscoveryLanguageOption.PortugueseOriginal;
            if (cmbDiscoveryLanguage.SelectedIndex == 2) return DiscoveryLanguageOption.PortugueseOriginalOrProbablyDubbed;
            return DiscoveryLanguageOption.Any;
        }

        private int GetSelectedDiscoveryType()
        {
            return cmbMediaType.SelectedIndex;
        }

        private async void btnDiscover_Click(object sender, EventArgs e)
        {
            await ExecuteDiscoveryAsync();
        }

        private async Task ExecuteDiscoveryAsync()
        {
            BeginDiscoveryOperation();
            var version = discoveryOperationVersion;
            var genre = GetSelectedDiscoveryGenre();
            var sort = GetSelectedDiscoverySort();
            var type = GetSelectedDiscoveryType();
            var language = GetSelectedDiscoveryLanguage();
            isLoadingDiscovery = true;
            btnDiscover.Enabled = false;
            toolStripStatusLabelMessage.Text = "Descobrindo resultados...";
            UpdateLoadMoreButton();
            try
            {
                var pageResult = await GetDiscoveryPageAsync(type, genre, sort, language, 1, discoveryCancellation.Token);
                if (!IsCurrentDiscoveryOperation(version)) return;
                discoveryAccumulatedItems = ApplyDiscoveryFilters(pageResult.Items, language, sort);
                discoveryCurrentPage = Math.Max(1, pageResult.Page);
                discoveryTotalPages = Math.Max(0, pageResult.TotalPages);
                discoveryTotalResults = Math.Max(0, pageResult.TotalResults);
                RenderDiscoveryResults(sort);
                toolStripStatusLabelMessage.Text = discoveryAccumulatedItems.Count == 0
                    ? "Nenhum resultado encontrado."
                    : string.Format("{0} resultado(s) carregado(s) de {1}.", discoveryAccumulatedItems.Count, discoveryTotalResults > 0 ? discoveryTotalResults : discoveryAccumulatedItems.Count);
                if (language == DiscoveryLanguageOption.PortugueseOriginalOrProbablyDubbed)
                {
                    toolStripStatusLabelMessage.Text += " Dublagem é estimada e pode variar por serviço.";
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (InvalidOperationException ex)
            {
                if (IsCurrentDiscoveryOperation(version))
                {
                    toolStripStatusLabelMessage.Text = ex.Message == "Configure sua chave do TMDb em Configurações." ? "Configure a chave do TMDb em Configurações." : "Erro ao consultar o TMDb.";
                }
            }
            catch (Exception)
            {
                if (IsCurrentDiscoveryOperation(version)) toolStripStatusLabelMessage.Text = "Erro ao consultar o TMDb.";
            }
            finally
            {
                if (IsCurrentDiscoveryOperation(version))
                {
                    isLoadingDiscovery = false;
                    btnDiscover.Enabled = true;
                    UpdateLoadMoreButton();
                }
            }
        }

        private void BeginDiscoveryOperation()
        {
            InvalidateDiscoveryOperation();
            discoveryCancellation = new CancellationTokenSource();
            discoveryAccumulatedItems = new List<MediaItem>();
            discoveryCurrentPage = 0;
            discoveryTotalPages = 0;
            discoveryTotalResults = 0;
            discoveryReleaseDateTo = DateTime.Today;
            discoveryReleaseDateFrom = discoveryReleaseDateTo.Value.AddMonths(-12);
        }

        private void InvalidateDiscoveryOperation()
        {
            discoveryOperationVersion++;
            if (discoveryCancellation != null)
            {
                discoveryCancellation.Cancel();
                discoveryCancellation.Dispose();
                discoveryCancellation = null;
            }
            isLoadingDiscovery = false;
        }

        private bool IsCurrentDiscoveryOperation(int version)
        {
            return version == discoveryOperationVersion && currentView == MainViewMode.Discovery && !IsDisposed && !Disposing;
        }

        private async Task<PagedMediaResult> GetDiscoveryPageAsync(int type, DiscoveryGenreOption genre, DiscoverySortOption sort, DiscoveryLanguageOption language, int page, CancellationToken token)
        {
            if (type == 1)
            {
                if (genre != null && !genre.MovieGenreId.HasValue) return new PagedMediaResult();
                return await tmdbService.DiscoverMoviesPageAsync(genre == null ? (int?)null : genre.MovieGenreId, sort, page, token, language, discoveryReleaseDateFrom, discoveryReleaseDateTo);
            }
            if (type == 2)
            {
                if (genre != null && !genre.TvGenreId.HasValue) return new PagedMediaResult();
                return await tmdbService.DiscoverTvPageAsync(genre == null ? (int?)null : genre.TvGenreId, sort, page, token, language, discoveryReleaseDateFrom, discoveryReleaseDateTo);
            }

            var tasks = new List<Task<PagedMediaResult>>();
            if (genre == null || genre.MovieGenreId.HasValue)
            {
                tasks.Add(tmdbService.DiscoverMoviesPageAsync(genre == null ? (int?)null : genre.MovieGenreId, sort, page, token, language, discoveryReleaseDateFrom, discoveryReleaseDateTo));
            }
            if (genre == null || genre.TvGenreId.HasValue)
            {
                tasks.Add(tmdbService.DiscoverTvPageAsync(genre == null ? (int?)null : genre.TvGenreId, sort, page, token, language, discoveryReleaseDateFrom, discoveryReleaseDateTo));
            }
            var pages = await Task.WhenAll(tasks);
            var result = new PagedMediaResult
            {
                Page = page,
                TotalPages = pages.Length == 0 ? 0 : pages.Max(item => item.TotalPages),
                TotalResults = pages.Sum(item => item.TotalResults)
            };
            foreach (var pageResult in pages)
            {
                AppendDistinct(result.Items, pageResult.Items);
            }
            return result;
        }

        private static void AppendDistinct(ICollection<MediaItem> target, IEnumerable<MediaItem> items)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                if (item == null || target.Any(existing => existing.Id == item.Id && existing.MediaType == item.MediaType)) continue;
                target.Add(item);
            }
        }

        private List<MediaItem> ApplyDiscoveryFilters(IEnumerable<MediaItem> items, DiscoveryLanguageOption language, DiscoverySortOption sort)
        {
            var filtered = ApplyDiscoveryLanguageFilter(items, language);
            if (sort != DiscoverySortOption.ReleaseDate) return filtered;
            var from = discoveryReleaseDateFrom.Value;
            var to = discoveryReleaseDateTo.Value;
            return filtered.Where(item => item != null && item.ReleaseDate.HasValue && item.ReleaseDate.Value.Date >= from.Date && item.ReleaseDate.Value.Date <= to.Date).ToList();
        }

        private static List<MediaItem> ApplyDiscoveryLanguageFilter(IEnumerable<MediaItem> items, DiscoveryLanguageOption language)
        {
            var source = items ?? Enumerable.Empty<MediaItem>();
            if (language != DiscoveryLanguageOption.PortugueseOriginalOrProbablyDubbed) return source.ToList();
            return source.Where(MatchesPortugueseOriginalOrExplicitDubbingSignal).ToList();
        }

        private static bool MatchesPortugueseOriginalOrExplicitDubbingSignal(MediaItem item)
        {
            if (item == null) return false;
            if (string.Equals(item.OriginalLanguage, "pt", StringComparison.OrdinalIgnoreCase)) return true;
            var text = ((item.Title ?? string.Empty) + " " + (item.Overview ?? string.Empty)).ToLowerInvariant();
            return text.Contains("dublado em portugues") || text.Contains("dublado em português") || text.Contains("dublada em portugues") || text.Contains("dublada em português") ||
                text.Contains("dublagem em portugues") || text.Contains("dublagem em português") || text.Contains("dublado pt-br") ||
                text.Contains("dublada pt-br") || text.Contains("dublagem pt-br") ||
                text.Contains("versao brasileira") || text.Contains("versão brasileira") || text.Contains("portugues brasileiro") || text.Contains("português brasileiro");
        }

        private void RenderDiscoveryResults(DiscoverySortOption sort)
        {
            var items = discoveryAccumulatedItems ?? new List<MediaItem>();
            IEnumerable<MediaItem> ordered = items;
            if (sort == DiscoverySortOption.ReleaseDate) ordered = items.OrderByDescending(item => item.ReleaseDate ?? DateTime.MinValue).ThenBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase);
            else if (sort == DiscoverySortOption.VoteAverage) ordered = items.OrderByDescending(item => item.Rating ?? 0).ThenBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase);
            else ordered = items.OrderBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase);
            ClearResults();
            foreach (var item in ordered) flowResults.Controls.Add(CreateMediaCard(item));
        }

        private async Task LoadMoreDiscoveryAsync()
        {
            if (isLoadingMore || isLoadingDiscovery || discoveryCancellation == null || discoveryCurrentPage < 1 || discoveryCurrentPage >= discoveryTotalPages) return;
            var version = discoveryOperationVersion;
            var nextPage = discoveryCurrentPage + 1;
            isLoadingMore = true;
            btnLoadMore.Enabled = false;
            toolStripStatusLabelMessage.Text = "Carregando mais resultados...";
            try
            {
                var pageResult = await GetDiscoveryPageAsync(cmbMediaType.SelectedIndex, GetSelectedDiscoveryGenre(), GetSelectedDiscoverySort(), GetSelectedDiscoveryLanguage(), nextPage, discoveryCancellation.Token);
                if (!IsCurrentDiscoveryOperation(version)) return;
                AppendDistinct(discoveryAccumulatedItems, ApplyDiscoveryFilters(pageResult.Items, GetSelectedDiscoveryLanguage(), GetSelectedDiscoverySort()));
                discoveryCurrentPage = pageResult.Page < nextPage ? nextPage : pageResult.Page;
                discoveryTotalPages = Math.Max(discoveryTotalPages, pageResult.TotalPages);
                if (pageResult.TotalResults > 0) discoveryTotalResults = pageResult.TotalResults;
                RenderDiscoveryResults(GetSelectedDiscoverySort());
                toolStripStatusLabelMessage.Text = string.Format("{0} resultado(s) carregado(s) de {1}.", discoveryAccumulatedItems.Count, discoveryTotalResults > 0 ? discoveryTotalResults : discoveryAccumulatedItems.Count);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                if (IsCurrentDiscoveryOperation(version)) toolStripStatusLabelMessage.Text = "Não foi possível carregar mais resultados.";
            }
            finally
            {
                if (IsCurrentDiscoveryOperation(version))
                {
                    isLoadingMore = false;
                    UpdateLoadMoreButton();
                }
            }
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
            BeginNewSearch(query);
            var operationVersion = searchOperationVersion;
            btnSearch.Enabled = false;
            chkOwnedOnly.Enabled = false;
            toolStripStatusLabelMessage.Text = "Pesquisando...";

            try
            {
                var pageResult = await tmdbService.SearchPageAsync(query, 1);
                if (!IsCurrentSearchOperation(operationVersion))
                {
                    return;
                }

                currentSearchPage = pageResult == null ? 1 : Math.Max(1, pageResult.Page);
                totalSearchPages = pageResult == null ? 0 : Math.Max(0, pageResult.TotalPages);
                totalSearchResults = pageResult == null ? 0 : Math.Max(0, pageResult.TotalResults);
                var historySaveFailed = false;

                try
                {
                    searchHistoryService.Add(query, GetSelectedSearchType());
                }
                catch (SearchHistoryPersistenceException)
                {
                    historySaveFailed = true;
                }

                lastSearchResults = pageResult == null || pageResult.Items == null
                    ? new List<MediaItem>()
                    : pageResult.Items;
                UpdateYearOptions(lastSearchResults);
                UpdateGenreOptions(lastSearchResults);
                var filteredResults = await GetCurrentSearchResultsAsync(true);
                if (filteredResults == null)
                {
                    return;
                }

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
                    toolStripStatusLabelMessage.Text = chkOwnedOnly.Checked && lastSearchResults.Count > 0
                        ? "Nenhum título encontrado nos serviços habilitados."
                        : "Nenhum resultado encontrado com os filtros selecionados.";
                }
                else
                {
                    toolStripStatusLabelMessage.Text = string.Format(
                        "{0} resultado(s) carregado(s) de {1}.",
                        filteredResults.Count,
                        totalSearchResults > 0 ? totalSearchResults : lastSearchResults.Count);
                }

                UpdateLoadMoreButton();
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
                chkOwnedOnly.Enabled = true;
                UpdateLoadMoreButton();
            }
        }

        private void BeginNewSearch(string query)
        {
            searchOperationVersion++;
            CancelFilterOperation();
            currentSearchQuery = query;
            currentSearchPage = 0;
            totalSearchPages = 0;
            totalSearchResults = 0;
            isLoadingMore = false;
            lastSearchResults = new List<MediaItem>();
            lastOwnedFilterResults = null;
            lastOwnedFilterCandidateKey = null;
            UpdateLoadMoreButton();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            searchOperationVersion++;
            CancelFilterOperation();
            isLoadingMore = false;
            InvalidateDiscoveryOperation();
            currentView = MainViewMode.History;
        }

        private bool IsCurrentSearchOperation(int operationVersion)
        {
            return operationVersion == searchOperationVersion &&
                currentView == MainViewMode.Search &&
                !string.IsNullOrWhiteSpace(currentSearchQuery) &&
                !IsDisposed && !Disposing;
        }

        private async void btnLoadMore_Click(object sender, EventArgs e)
        {
            if (currentView == MainViewMode.Discovery)
            {
                await LoadMoreDiscoveryAsync();
                return;
            }

            if (isLoadingMore || currentView != MainViewMode.Search ||
                string.IsNullOrWhiteSpace(currentSearchQuery) ||
                currentSearchPage < 1 || currentSearchPage >= totalSearchPages)
            {
                return;
            }

            var operationVersion = searchOperationVersion;
            var nextPage = currentSearchPage + 1;
            isLoadingMore = true;
            UpdateLoadMoreButton();
            chkOwnedOnly.Enabled = false;
            toolStripStatusLabelMessage.Text = "Carregando mais resultados...";

            try
            {
                var pageResult = await tmdbService.SearchPageAsync(currentSearchQuery, nextPage);
                if (operationVersion != searchOperationVersion || currentView != MainViewMode.Search)
                {
                    return;
                }

                if (pageResult == null)
                {
                    toolStripStatusLabelMessage.Text = "Não foi possível carregar mais resultados.";
                    return;
                }

                AppendDistinctResults(pageResult.Items);
                currentSearchPage = pageResult.Page < nextPage ? nextPage : pageResult.Page;
                if (pageResult.TotalPages > 0)
                {
                    totalSearchPages = pageResult.TotalPages;
                }

                if (pageResult.TotalResults > 0)
                {
                    totalSearchResults = pageResult.TotalResults;
                }

                UpdateYearOptions(lastSearchResults);
                UpdateGenreOptions(lastSearchResults);
                var filteredResults = await GetCurrentSearchResultsForAdditionalPageAsync(pageResult.Items);
                if (operationVersion != searchOperationVersion || filteredResults == null)
                {
                    return;
                }

                RenderSearchResults(filteredResults);
                toolStripStatusLabelMessage.Text = string.Format(
                    "{0} resultado(s) carregado(s) de {1}.",
                    lastSearchResults.Count,
                    totalSearchResults > 0 ? totalSearchResults : lastSearchResults.Count);
            }
            catch (Exception)
            {
                if (operationVersion == searchOperationVersion)
                {
                    toolStripStatusLabelMessage.Text = "Não foi possível carregar mais resultados.";
                }
            }
            finally
            {
                if (operationVersion == searchOperationVersion)
                {
                    isLoadingMore = false;
                    chkOwnedOnly.Enabled = true;
                    UpdateLoadMoreButton();
                }
            }
        }

        private void AppendDistinctResults(IList<MediaItem> newItems)
        {
            if (newItems == null)
            {
                return;
            }

            foreach (var item in newItems)
            {
                if (item == null || lastSearchResults.Any(existing =>
                    string.Equals(existing.Id, item.Id, StringComparison.OrdinalIgnoreCase) &&
                    existing.MediaType == item.MediaType))
                {
                    continue;
                }

                lastSearchResults.Add(item);
            }
        }

        private void UpdateLoadMoreButton()
        {
            var hasNextPage = currentView == MainViewMode.Search
                ? currentSearchPage > 0 && totalSearchPages > currentSearchPage
                : currentView == MainViewMode.Discovery && discoveryCurrentPage > 0 && discoveryTotalPages > discoveryCurrentPage;
            btnLoadMore.Visible = hasNextPage;
            btnLoadMore.Enabled = hasNextPage && !isLoadingMore && !isLoadingDiscovery;
        }

        private async void chkOwnedOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingDiscoveryControls || currentView != MainViewMode.Search || isLoadingMore || !btnSearch.Enabled || lastSearchResults == null)
            {
                return;
            }

            btnSearch.Enabled = false;
            chkOwnedOnly.Enabled = false;
            try
            {
                var filteredResults = await GetCurrentSearchResultsAsync(true);
                if (filteredResults == null || currentView != MainViewMode.Search)
                {
                    return;
                }

                ClearResults();
                foreach (var item in filteredResults)
                {
                    flowResults.Controls.Add(CreateMediaCard(item));
                }

                toolStripStatusLabelMessage.Text = filteredResults.Count == 0 && lastSearchResults.Count > 0
                    ? "Nenhum título encontrado nos serviços habilitados."
                    : filteredResults.Count == 0
                        ? "Nenhum resultado encontrado com os filtros selecionados."
                        : string.Format("{0} resultado(s) encontrado(s).", filteredResults.Count);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                if (!IsDisposed && !Disposing)
                {
                    toolStripStatusLabelMessage.Text = "N\u00e3o foi poss\u00edvel aplicar o filtro de servi\u00e7os.";
                }
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    btnSearch.Enabled = true;
                    chkOwnedOnly.Enabled = true;
                }
            }
        }

        private async void SearchViewFilter_Changed(object sender, EventArgs e)
        {
            if (isUpdatingDiscoveryControls || currentView != MainViewMode.Search || isLoadingMore || !btnSearch.Enabled ||
                lastSearchResults == null || lastSearchResults.Count == 0)
            {
                return;
            }

            btnSearch.Enabled = false;
            chkOwnedOnly.Enabled = false;
            try
            {
                var filteredResults = await GetCurrentSearchResultsAsync(false);
                if (filteredResults == null || currentView != MainViewMode.Search)
                {
                    return;
                }

                RenderSearchResults(filteredResults);
                toolStripStatusLabelMessage.Text = filteredResults.Count == 0
                    ? "Nenhum resultado encontrado com os filtros selecionados."
                    : string.Format("{0} resultado(s) encontrado(s).", filteredResults.Count);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                if (!IsDisposed && !Disposing)
                {
                    toolStripStatusLabelMessage.Text = "N\u00e3o foi poss\u00edvel aplicar os filtros.";
                }
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    btnSearch.Enabled = true;
                    chkOwnedOnly.Enabled = true;
                }
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

        private async Task<List<MediaItem>> GetCurrentSearchResultsAsync(bool forceOwnedFilterRefresh)
        {
            var candidates = GetCurrentSearchCandidates();
            if (!chkOwnedOnly.Checked)
            {
                lastOwnedFilterResults = null;
                lastOwnedFilterCandidateKey = null;
                return SortSearchResults(candidates);
            }

            var candidateKey = CreateCandidateKey(candidates);
            if (!forceOwnedFilterRefresh && lastOwnedFilterResults != null &&
                string.Equals(candidateKey, lastOwnedFilterCandidateKey, StringComparison.Ordinal))
            {
                return SortSearchResults(new List<MediaItem>(lastOwnedFilterResults));
            }

            var ownedResults = await ApplyOwnedServicesFilterAsync(candidates);
            if (ownedResults == null)
            {
                return null;
            }

            lastOwnedFilterResults = new List<MediaItem>(ownedResults);
            lastOwnedFilterCandidateKey = candidateKey;
            return SortSearchResults(ownedResults);
        }

        private List<MediaItem> GetCurrentSearchCandidates()
        {
            return GetCurrentSearchCandidates(lastSearchResults);
        }

        private List<MediaItem> GetCurrentSearchCandidates(IList<MediaItem> source)
        {
            var candidates = FilterByMediaType((source ?? new List<MediaItem>()).ToList());
            int selectedYear;
            if (!TryGetSelectedYear(out selectedYear))
            {
                return FilterBySelectedGenre(candidates);
            }

            return FilterBySelectedGenre(candidates
                .Where(item => item.Year.HasValue && item.Year.Value == selectedYear)
                .ToList());
        }

        private async Task<List<MediaItem>> GetCurrentSearchResultsForAdditionalPageAsync(
            IList<MediaItem> newItems)
        {
            if (!chkOwnedOnly.Checked)
            {
                lastOwnedFilterResults = null;
                lastOwnedFilterCandidateKey = null;
                return SortSearchResults(GetCurrentSearchCandidates());
            }

            var newCandidates = GetCurrentSearchCandidates(newItems);
            var newOwnedResults = await ApplyOwnedServicesFilterAsync(newCandidates);
            if (newOwnedResults == null)
            {
                return null;
            }

            var combinedOwnedResults = new List<MediaItem>(lastOwnedFilterResults ?? new List<MediaItem>());
            foreach (var item in newOwnedResults)
            {
                if (!combinedOwnedResults.Any(existing =>
                    string.Equals(existing.Id, item.Id, StringComparison.OrdinalIgnoreCase) &&
                    existing.MediaType == item.MediaType))
                {
                    combinedOwnedResults.Add(item);
                }
            }

            lastOwnedFilterResults = combinedOwnedResults;
            lastOwnedFilterCandidateKey = CreateCandidateKey(GetCurrentSearchCandidates());
            return SortSearchResults(combinedOwnedResults);
        }

        private List<MediaItem> FilterBySelectedGenre(IList<MediaItem> candidates)
        {
            var selectedGenre = GetSelectedGenre();
            if (string.IsNullOrWhiteSpace(selectedGenre))
            {
                return new List<MediaItem>(candidates ?? new List<MediaItem>());
            }

            return (candidates ?? new List<MediaItem>())
                .Where(item => item.Genres != null && item.Genres.Any(genre =>
                    string.Equals(genre, selectedGenre, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        private string GetSelectedGenre()
        {
            if (cmbGenre.SelectedIndex <= 0 || cmbGenre.SelectedItem == null)
            {
                return null;
            }

            return cmbGenre.SelectedItem.ToString();
        }

        private bool TryGetSelectedYear(out int year)
        {
            year = 0;
            if (cmbYear.SelectedIndex <= 0 || cmbYear.SelectedItem == null)
            {
                return false;
            }

            return int.TryParse(cmbYear.SelectedItem.ToString(), out year);
        }

        private void UpdateYearOptions(IList<MediaItem> results)
        {
            int previousYear;
            var hadPreviousYear = TryGetSelectedYear(out previousYear);

            cmbYear.BeginUpdate();
            try
            {
                cmbYear.Items.Clear();
                cmbYear.Items.Add("Todos os anos");
                foreach (var year in (results ?? new List<MediaItem>())
                    .Where(item => item != null && item.Year.HasValue)
                    .Select(item => item.Year.Value)
                    .Distinct()
                    .OrderByDescending(year => year))
                {
                    cmbYear.Items.Add(year.ToString());
                }

                var selectedIndex = hadPreviousYear
                    ? cmbYear.Items.IndexOf(previousYear.ToString())
                    : -1;
                cmbYear.SelectedIndex = selectedIndex >= 1 ? selectedIndex : 0;
            }
            finally
            {
                cmbYear.EndUpdate();
            }
        }

        private void UpdateGenreOptions(IList<MediaItem> results)
        {
            var previousGenre = GetSelectedGenre();
            var genres = (results ?? new List<MediaItem>())
                .Where(item => item != null && item.Genres != null)
                .SelectMany(item => item.Genres)
                .Where(genre => !string.IsNullOrWhiteSpace(genre))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(genre => genre, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            cmbGenre.BeginUpdate();
            try
            {
                cmbGenre.Items.Clear();
                cmbGenre.Items.Add("Todos os gêneros");
                foreach (var genre in genres)
                {
                    cmbGenre.Items.Add(genre);
                }

                var selectedIndex = genres.FindIndex(genre =>
                    string.Equals(genre, previousGenre, StringComparison.OrdinalIgnoreCase));
                cmbGenre.SelectedIndex = selectedIndex >= 0 ? selectedIndex + 1 : 0;
            }
            finally
            {
                cmbGenre.EndUpdate();
            }
        }

        private static string CreateCandidateKey(IList<MediaItem> candidates)
        {
            return string.Join("|", (candidates ?? new List<MediaItem>())
                .Select(item => string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0}:{1}",
                    item.Id ?? string.Empty,
                    item.MediaType)));
        }

        private List<MediaItem> SortSearchResults(IList<MediaItem> results)
        {
            var source = results ?? new List<MediaItem>();
            switch (cmbSort.SelectedIndex)
            {
                case 1:
                    return source
                        .OrderByDescending(item => item.Year.HasValue)
                        .ThenByDescending(item => item.Year ?? 0)
                        .ThenBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();
                case 2:
                    return source
                        .OrderByDescending(item => item.Rating.HasValue)
                        .ThenByDescending(item => item.Rating ?? 0D)
                        .ThenBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();
                default:
                    return source
                        .OrderBy(item => item.Title ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();
            }
        }

        private void RenderSearchResults(IList<MediaItem> results)
        {
            ClearResults();
            foreach (var item in results ?? new List<MediaItem>())
            {
                flowResults.Controls.Add(CreateMediaCard(item));
            }
        }

        private async Task<List<MediaItem>> ApplyOwnedServicesFilterAsync(IList<MediaItem> candidates)
        {
            CancelFilterOperation();
            var operationVersion = ++filterVersion;
            filterCancellation = new CancellationTokenSource();
            var cancellationToken = filterCancellation.Token;

            try
            {
                if (!chkOwnedOnly.Checked)
                {
                    return new List<MediaItem>(candidates ?? new List<MediaItem>());
                }

                toolStripStatusLabelMessage.Text = "Verificando serviços disponíveis...";
                var compatibleResults = new List<MediaItem>();
                var syncRoot = new object();
                var failures = 0;
                using (var semaphore = new SemaphoreSlim(4, 4))
                {
                    var tasks = new List<Task>();
                    if (candidates != null)
                    {
                        foreach (var item in candidates)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            tasks.Add(CheckOwnedAvailabilityAsync(
                                item,
                                semaphore,
                                cancellationToken,
                                compatibleResults,
                                syncRoot,
                                () => Interlocked.Increment(ref failures)));
                        }
                    }

                    await Task.WhenAll(tasks);
                }

                if (cancellationToken.IsCancellationRequested || operationVersion != filterVersion)
                {
                    return null;
                }

                return compatibleResults;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            finally
            {
                if (operationVersion == filterVersion && filterCancellation != null)
                {
                    filterCancellation.Dispose();
                    filterCancellation = null;
                }
            }
        }

        private async Task CheckOwnedAvailabilityAsync(
            MediaItem item,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken,
            List<MediaItem> compatibleResults,
            object syncRoot,
            Action registerFailure)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var availability = await tmdbService.GetWatchProvidersAsync(item);
                if (availability != null && availability.Any(providerPreferenceService.IsAvailabilityCompatibleWithOwnedServices))
                {
                    lock (syncRoot)
                    {
                        compatibleResults.Add(item);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                registerFailure();
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void CancelFilterOperation()
        {
            filterVersion++;
            if (filterCancellation == null)
            {
                return;
            }

            filterCancellation.Cancel();
            filterCancellation.Dispose();
            filterCancellation = null;
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
            if (card == null || card.Media == null)
            {
                return;
            }

            using (var form = new MediaDetailsForm(
                card.Media,
                favoritesService,
                tmdbService,
                iniFileService,
                httpService,
                availabilityFallbackService))
            {
                form.ShowDialog(this);
            }

            var isFavorite = favoritesService.IsFavorite(card.Media);
            card.SetFavoriteState(isFavorite);
            if (!isFavorite && currentView == MainViewMode.Favorites)
            {
                flowResults.Controls.Remove(card);
                card.Dispose();
            }
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
            searchOperationVersion++;
            CancelFilterOperation();
            InvalidateDiscoveryOperation();
            ConfigureSearchControls();
            currentView = MainViewMode.Favorites;
            UpdateLoadMoreButton();
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
            searchOperationVersion++;
            CancelFilterOperation();
            InvalidateDiscoveryOperation();
            currentView = MainViewMode.Search;
            ConfigureSearchControls();
            UpdateLoadMoreButton();
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
            searchOperationVersion++;
            CancelFilterOperation();
            InvalidateDiscoveryOperation();
            currentView = MainViewMode.History;
            ConfigureSearchControls();
            UpdateLoadMoreButton();
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
