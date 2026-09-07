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
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbMediaType;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.ComboBox cmbSort;
        private System.Windows.Forms.CheckBox chkOwnedOnly;
        private System.Windows.Forms.FlowLayoutPanel flowResults;
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
            panelTop = new System.Windows.Forms.Panel();
            txtSearch = new System.Windows.Forms.TextBox();
            btnSearch = new System.Windows.Forms.Button();
            cmbMediaType = new System.Windows.Forms.ComboBox();
            cmbGenre = new System.Windows.Forms.ComboBox();
            cmbYear = new System.Windows.Forms.ComboBox();
            cmbSort = new System.Windows.Forms.ComboBox();
            chkOwnedOnly = new System.Windows.Forms.CheckBox();
            flowResults = new System.Windows.Forms.FlowLayoutPanel();
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            panelSidebar.SuspendLayout();
            panelTop.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // panelSidebar
            panelSidebar.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            panelSidebar.Controls.Add(btnSettings);
            panelSidebar.Controls.Add(btnHistory);
            panelSidebar.Controls.Add(btnFavorites);
            panelSidebar.Controls.Add(btnSearchNavigation);
            panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            panelSidebar.Padding = new System.Windows.Forms.Padding(10);
            panelSidebar.Size = new System.Drawing.Size(160, 689);
            // navigation buttons
            ConfigureNavigationButton(btnSearchNavigation, "Pesquisa", 20);
            ConfigureNavigationButton(btnFavorites, "Favoritos", 65);
            ConfigureNavigationButton(btnHistory, "Histórico", 110);
            ConfigureNavigationButton(btnSettings, "Configurações", 155);
            btnSearchNavigation.Click += new System.EventHandler(btnSearchNavigation_Click);
            btnFavorites.Click += new System.EventHandler(btnFavorites_Click);
            btnHistory.Click += new System.EventHandler(btnHistory_Click);
            btnSettings.Click += new System.EventHandler(btnSettings_Click);
            // panelTop
            panelTop.Controls.Add(chkOwnedOnly);
            panelTop.Controls.Add(cmbSort);
            panelTop.Controls.Add(cmbYear);
            panelTop.Controls.Add(cmbGenre);
            panelTop.Controls.Add(cmbMediaType);
            panelTop.Controls.Add(btnSearch);
            panelTop.Controls.Add(txtSearch);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Height = 105;
            panelTop.Padding = new System.Windows.Forms.Padding(12);
            // search and filters
            txtSearch.Location = new System.Drawing.Point(15, 15);
            txtSearch.Size = new System.Drawing.Size(440, 23);
            btnSearch.Location = new System.Drawing.Point(465, 14);
            btnSearch.Size = new System.Drawing.Size(105, 26);
            btnSearch.Text = "Pesquisar";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += new System.EventHandler(btnSearch_Click);
            ConfigureCombo(cmbMediaType, "Todos", "Filmes", "Séries", 590, "Tipo");
            ConfigureCombo(cmbGenre, "Todos os gêneros", 710, "Gênero");
            ConfigureCombo(cmbYear, "Todos os anos", 855, "Ano");
            ConfigureCombo(cmbSort, "Título", "Ano", "Nota", 1000, "Ordenação");
            chkOwnedOnly.AutoSize = true;
            chkOwnedOnly.Location = new System.Drawing.Point(15, 62);
            chkOwnedOnly.Text = "Mostrar somente streamings que eu possuo";
            chkOwnedOnly.CheckedChanged += new System.EventHandler(chkOwnedOnly_CheckedChanged);
            // flowResults
            flowResults.AutoScroll = true;
            flowResults.BackColor = System.Drawing.Color.WhiteSmoke;
            flowResults.Dock = System.Windows.Forms.DockStyle.Fill;
            flowResults.Padding = new System.Windows.Forms.Padding(12);
            // status
            statusStrip.Items.Add(toolStripStatusLabelMessage);
            statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            toolStripStatusLabelMessage.Text = "Pronto.";
            // form
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 750);
            Controls.Add(flowResults);
            Controls.Add(panelTop);
            Controls.Add(panelSidebar);
            Controls.Add(statusStrip);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            MinimumSize = new System.Drawing.Size(900, 600);
            Text = "StreamFinder - Filmes e Séries";
            panelSidebar.ResumeLayout(false);
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigureNavigationButton(System.Windows.Forms.Button button, string text, int top)
        {
            button.Location = new System.Drawing.Point(10, top);
            button.Size = new System.Drawing.Size(140, 35);
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
            combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            combo.Items.AddRange(items);
            combo.SelectedIndex = 0;
            combo.Location = new System.Drawing.Point(left, 15);
            combo.Size = new System.Drawing.Size(130, 23);
            combo.AccessibleName = placeholder;
        }
    }
}
