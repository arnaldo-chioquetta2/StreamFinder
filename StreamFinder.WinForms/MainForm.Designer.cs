namespace StreamFinder.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnSearchNavigation;
        private System.Windows.Forms.Button btnFavorites;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.TableLayoutPanel panelTop;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbMediaType;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.ComboBox cmbSort;
        private System.Windows.Forms.CheckBox chkOwnedOnly;
        private System.Windows.Forms.FlowLayoutPanel flowResults;
        private System.Windows.Forms.TableLayoutPanel panelLoadMore;
        private System.Windows.Forms.Button btnLoadMore;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelSidebar = new System.Windows.Forms.Panel();
            btnSearchNavigation = new System.Windows.Forms.Button();
            btnFavorites = new System.Windows.Forms.Button();
            btnHistory = new System.Windows.Forms.Button();
            btnSettings = new System.Windows.Forms.Button();
            panelTop = new System.Windows.Forms.TableLayoutPanel();
            txtSearch = new System.Windows.Forms.TextBox();
            btnSearch = new System.Windows.Forms.Button();
            cmbMediaType = new System.Windows.Forms.ComboBox();
            cmbGenre = new System.Windows.Forms.ComboBox();
            cmbYear = new System.Windows.Forms.ComboBox();
            cmbSort = new System.Windows.Forms.ComboBox();
            chkOwnedOnly = new System.Windows.Forms.CheckBox();
            flowResults = new System.Windows.Forms.FlowLayoutPanel();
            panelLoadMore = new System.Windows.Forms.TableLayoutPanel();
            btnLoadMore = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            panelSidebar.SuspendLayout();
            panelTop.SuspendLayout();
            panelLoadMore.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            panelSidebar.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            panelSidebar.Controls.Add(btnSettings);
            panelSidebar.Controls.Add(btnHistory);
            panelSidebar.Controls.Add(btnFavorites);
            panelSidebar.Controls.Add(btnSearchNavigation);
            panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            panelSidebar.Padding = new System.Windows.Forms.Padding(10);
            panelSidebar.Size = new System.Drawing.Size(180, 739);
            ConfigureNavigationButton(btnSearchNavigation, "Pesquisa", 20);
            ConfigureNavigationButton(btnFavorites, "Favoritos", 72);
            ConfigureNavigationButton(btnHistory, "Hist\u00f3rico", 124);
            ConfigureNavigationButton(btnSettings, "Configura\u00e7\u00f5es", 176);
            btnSearchNavigation.Click += new System.EventHandler(btnSearchNavigation_Click);
            btnFavorites.Click += new System.EventHandler(btnFavorites_Click);
            btnHistory.Click += new System.EventHandler(btnHistory_Click);
            btnSettings.Click += new System.EventHandler(btnSettings_Click);
            panelTop.ColumnCount = 5;
            panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            panelTop.RowCount = 3;
            panelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            panelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            panelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Height = 128;
            panelTop.Padding = new System.Windows.Forms.Padding(12, 10, 12, 8);
            panelTop.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            panelTop.Controls.Add(txtSearch, 0, 0);
            panelTop.SetColumnSpan(txtSearch, 4);
            panelTop.Controls.Add(btnSearch, 4, 0);
            txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtSearch.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            btnSearch.Text = "Pesquisar";
            btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnSearch.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += new System.EventHandler(btnSearch_Click);
            ConfigureCombo(cmbMediaType, "Todos", "Filmes", "S\u00e9ries", 0, "Tipo");
            ConfigureCombo(cmbGenre, "Todos os g\u00eaneros", 0, "G\u00eanero");
            ConfigureCombo(cmbYear, "Todos os anos", 0, "Ano");
            ConfigureCombo(cmbSort, "T\u00edtulo", "Ano", "Nota", 0, "Ordena\u00e7\u00e3o");
            panelTop.Controls.Add(cmbMediaType, 0, 1);
            panelTop.Controls.Add(cmbGenre, 1, 1);
            panelTop.SetColumnSpan(cmbGenre, 2);
            panelTop.Controls.Add(cmbYear, 3, 1);
            panelTop.Controls.Add(cmbSort, 4, 1);
            cmbGenre.SelectedIndexChanged += new System.EventHandler(SearchViewFilter_Changed);
            cmbYear.SelectedIndexChanged += new System.EventHandler(SearchViewFilter_Changed);
            cmbSort.SelectedIndexChanged += new System.EventHandler(SearchViewFilter_Changed);
            chkOwnedOnly.AutoSize = true;
            chkOwnedOnly.Anchor = System.Windows.Forms.AnchorStyles.Left;
            chkOwnedOnly.Font = new System.Drawing.Font("Segoe UI", 10F);
            chkOwnedOnly.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            chkOwnedOnly.Text = "Mostrar somente streamings que eu possuo";
            panelTop.Controls.Add(chkOwnedOnly, 0, 2);
            panelTop.SetColumnSpan(chkOwnedOnly, 5);
            chkOwnedOnly.CheckedChanged += new System.EventHandler(chkOwnedOnly_CheckedChanged);
            flowResults.AutoScroll = true;
            flowResults.BackColor = System.Drawing.Color.WhiteSmoke;
            flowResults.Dock = System.Windows.Forms.DockStyle.Fill;
            flowResults.Padding = new System.Windows.Forms.Padding(12);
            panelLoadMore.ColumnCount = 3;
            panelLoadMore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelLoadMore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            panelLoadMore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelLoadMore.RowCount = 1;
            panelLoadMore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panelLoadMore.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelLoadMore.Height = 48;
            panelLoadMore.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            panelLoadMore.Visible = false;
            btnLoadMore.Anchor = System.Windows.Forms.AnchorStyles.None;
            btnLoadMore.AutoSize = true;
            btnLoadMore.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnLoadMore.MinimumSize = new System.Drawing.Size(170, 34);
            btnLoadMore.Text = "Carregar mais";
            btnLoadMore.UseVisualStyleBackColor = true;
            btnLoadMore.Click += new System.EventHandler(btnLoadMore_Click);
            panelLoadMore.Controls.Add(btnLoadMore, 1, 0);
            statusStrip.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            statusStrip.Items.Add(toolStripStatusLabelMessage);
            statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            toolStripStatusLabelMessage.Text = "Pronto.";
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 800);
            Controls.Add(flowResults);
            Controls.Add(panelLoadMore);
            Controls.Add(panelTop);
            Controls.Add(panelSidebar);
            Controls.Add(statusStrip);
            AcceptButton = btnSearch;
            Font = new System.Drawing.Font("Segoe UI", 10F);
            MinimumSize = new System.Drawing.Size(1000, 650);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "StreamFinder - Filmes e S\u00e9ries";
            panelSidebar.ResumeLayout(false);
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            panelLoadMore.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigureNavigationButton(System.Windows.Forms.Button button, string text, int top)
        {
            button.Location = new System.Drawing.Point(10, top);
            button.Size = new System.Drawing.Size(160, 42);
            button.Font = new System.Drawing.Font("Segoe UI", 10F);
            button.Text = text;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.White;
            button.BackColor = System.Drawing.Color.FromArgb(64, 64, 68);
            button.FlatAppearance.BorderSize = 0;
        }

        private static void ConfigureCombo(System.Windows.Forms.ComboBox combo, string firstItem, int left, string placeholder)
        {
            ConfigureCombo(combo, new[] { firstItem }, left, placeholder);
        }

        private static void ConfigureCombo(System.Windows.Forms.ComboBox combo, string first, string second, string third, int left, string placeholder)
        {
            ConfigureCombo(combo, new[] { first, second, third }, left, placeholder);
        }

        private static void ConfigureCombo(System.Windows.Forms.ComboBox combo, string[] items, int left, string placeholder)
        {
            combo.Dock = System.Windows.Forms.DockStyle.Fill;
            combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            combo.Font = new System.Drawing.Font("Segoe UI", 10F);
            combo.Items.AddRange(items);
            combo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            combo.SelectedIndex = 0;
            combo.AccessibleName = placeholder;
        }
    }
}
