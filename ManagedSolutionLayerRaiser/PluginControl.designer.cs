namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser
{
    partial class PluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.ToolStrip();
            this.tssPrimarySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tssSecondarySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            this.solutionsGridView = new System.Windows.Forms.DataGridView();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tsbLoadManagedSolutions = new System.Windows.Forms.ToolStripButton();
            this.tsbRaiseManagedSolution = new System.Windows.Forms.ToolStripButton();
            this.tslFilter = new System.Windows.Forms.ToolStripLabel();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.solutionsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssPrimarySeparator,
            this.tsbLoadManagedSolutions,
            this.tsbRaiseManagedSolution,
            this.tssSecondarySeparator,
            this.tslFilter,
            this.tstbSearch,
            this.tsbSearch});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1406, 31);
            this.mainMenu.TabIndex = 4;
            this.mainMenu.Text = "mainMenu";
            // 
            // tssPrimarySeparator
            // 
            this.tssPrimarySeparator.Name = "tssPrimarySeparator";
            this.tssPrimarySeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // tssSecondarySeparator
            // 
            this.tssSecondarySeparator.Name = "tssSecondarySeparator";
            this.tssSecondarySeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // tstbSearch
            // 
            this.tstbSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tstbSearch.Name = "tstbSearch";
            this.tstbSearch.Size = new System.Drawing.Size(100, 31);
            this.tstbSearch.TextChanged += new System.EventHandler(this.tstbSearch_TextChanged);
            // 
            // solutionsGridView
            // 
            this.solutionsGridView.AllowUserToAddRows = false;
            this.solutionsGridView.AllowUserToDeleteRows = false;
            this.solutionsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.solutionsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.solutionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.solutionsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionsGridView.Location = new System.Drawing.Point(0, 31);
            this.solutionsGridView.MultiSelect = false;
            this.solutionsGridView.Name = "solutionsGridView";
            this.solutionsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.solutionsGridView.Size = new System.Drawing.Size(1406, 469);
            this.solutionsGridView.TabIndex = 5;
            this.solutionsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.solutionsGridView_CellContentClick);
            // 
            // tsbClose
            // 
            this.tsbClose.Image = global::XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.Properties.Resources.icons8_close_94;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(110, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tsbLoadManagedSolutions
            // 
            this.tsbLoadManagedSolutions.Image = global::XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.Properties.Resources.icons8_refresh_94;
            this.tsbLoadManagedSolutions.Name = "tsbLoadManagedSolutions";
            this.tsbLoadManagedSolutions.Size = new System.Drawing.Size(166, 28);
            this.tsbLoadManagedSolutions.Text = "Load Managed Solutions";
            this.tsbLoadManagedSolutions.Click += new System.EventHandler(this.tsbLoadManagedSolutions_Click);
            // 
            // tsbRaiseManagedSolution
            // 
            this.tsbRaiseManagedSolution.Image = global::XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.Properties.Resources.icons8_check_94;
            this.tsbRaiseManagedSolution.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRaiseManagedSolution.Name = "tsbRaiseManagedSolution";
            this.tsbRaiseManagedSolution.Size = new System.Drawing.Size(162, 28);
            this.tsbRaiseManagedSolution.Text = "Raise Managed Solution";
            this.tsbRaiseManagedSolution.Click += new System.EventHandler(this.tsbRaiseManagedSolution_Click);
            // 
            // tslFilter
            // 
            this.tslFilter.Image = global::XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.Properties.Resources.icons8_filter_48;
            this.tslFilter.Name = "tslFilter";
            this.tslFilter.Size = new System.Drawing.Size(115, 28);
            this.tslFilter.Text = "Filter Solutions: ";
            // 
            // tsbSearch
            // 
            this.tsbSearch.Image = global::XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.Properties.Resources.icons8_search_94;
            this.tsbSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Size = new System.Drawing.Size(70, 28);
            this.tsbSearch.Text = "Search";
            this.tsbSearch.Click += new System.EventHandler(this.tsbSearch_Click);
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.solutionsGridView);
            this.Controls.Add(this.mainMenu);
            this.Name = "PluginControl";
            this.Size = new System.Drawing.Size(1406, 500);
            this.Load += new System.EventHandler(this.ManagedSolutionLayerRaiserControl_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.solutionsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip mainMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbLoadManagedSolutions;
        private System.Windows.Forms.ToolStripSeparator tssPrimarySeparator;
        private System.Windows.Forms.DataGridView solutionsGridView;
        private System.Windows.Forms.ToolStripButton tsbRaiseManagedSolution;
        private System.Windows.Forms.ToolStripLabel tslFilter;
        private System.Windows.Forms.ToolStripSeparator tssSecondarySeparator;
        private System.Windows.Forms.ToolStripTextBox tstbSearch;
        private System.Windows.Forms.ToolStripButton tsbSearch;
    }
}
