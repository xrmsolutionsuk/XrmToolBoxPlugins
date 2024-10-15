using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Solution = XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic.Solution;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser
{
    public partial class PluginControl : PluginControlBase, IGitHubPlugin, IPayPalPlugin, IHelpPlugin
    {
        private Settings mySettings;
        private BindingSource solutionsBindingSource = new BindingSource();
        OpenFileDialog selectSolutionFileDialog;
        Solution selectedSolution = null;
        Publisher selectedPublisher = null;
        FilePathSettings fps;

        public string RepositoryName => "XrmToolBoxPlugins";

        public string UserName => "xrmsolutionsuk";

        public string DonationDescription => "Thank you for using the Managed Solution Layer Raiser XrmToolBox plugin. If you have found it helpful, please consider making a donation";

        public string EmailAccount => "info@xrmsolutionsuk.com";

        public string HelpUrl => "https://www.xrmsolutionsuk.com/blog/managed-solution-layer-raiser/";

        public PluginControl()
        {
            InitializeComponent();
        }

        private void ManagedSolutionLayerRaiserControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This tool performs solution imports and deletions in the target environment. Please ensure you use an account with system administrator privileges - ideally the account that you typically use for solution deployments", null);

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(typeof(XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.PluginControl), out mySettings))
            {
                mySettings = new Settings();
                mySettings.DefaultPathForTemporaryFiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                SettingsManager.Instance.Save(typeof(XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.PluginControl), mySettings);

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
            ExecuteMethod(LoadPublishers);
            ExecuteMethod(LoadManagedSolutions);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbLoadManagedSolutions_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(LoadPublishers);
            ExecuteMethod(LoadManagedSolutions);
        }

        private void ValidateSelectedSolution(DoWorkEventArgs args)
        {
            this.fps = FileStructureGenerator.GenerateFileStructure(selectSolutionFileDialog.FileName, mySettings);
            var solutionDoc = XDocument.Load(fps.SolutionXmlFileLocation);
            bool isValid = SolutionValidator.IsValid(solutionDoc, selectedSolution);
            if (!isValid)
            {
                args.Result = false;
                args.Cancel = true;
                throw new Exception("Supplied solution zip file does not match selected solution");
            }
            else
            {
                solutionDoc.XPathSelectElement("/ImportExportXml/SolutionManifest/UniqueName").Value = selectedSolution.UniqueName + "_Holding";
                solutionDoc.Save(fps.SolutionXmlFileLocation);
                if (File.Exists(fps.HoldingSolutionFilePath))
                {
                    File.Delete(fps.HoldingSolutionFilePath);
                }
                ZipFile.CreateFromDirectory(fps.ExtractedSolutionFolderPath, fps.HoldingSolutionFilePath);
                Directory.Delete(fps.ExtractedSolutionFolderPath, true);
                args.Result = true;
            }
        }

        private void DeleteHoldingSolution()
        {
            LogInfo(string.Format("Step 4 of 4: Deleting {0} holding managed solution", selectedSolution.OriginalSolutionName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 4 of 4: Deleting {0} holding managed solution", selectedSolution.OriginalSolutionName),
                Work = (worker, argsDeleteHolding) =>
                {
                    SolutionRaisingStatus raisingStatus = SolutionValidator.GetSolutionRaisingStatus(Service, selectedSolution.OriginalSolutionName);
                    if (raisingStatus == SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionReinstalled)
                    {
                        bool success = SolutionManager.DeleteSolution(Service, selectedSolution.OriginalSolutionName + "_Holding");
                        if (!success)
                        {
                            argsDeleteHolding.Result = false;
                            argsDeleteHolding.Cancel = true;
                            throw new Exception("Delete of holding solution failed or cancelled");
                        }
                        else
                        {
                            argsDeleteHolding.Result = true;
                            argsDeleteHolding.Cancel = false;
                        }
                    }
                    else
                    {
                        argsDeleteHolding.Result = true;
                        argsDeleteHolding.Cancel = false;
                    }
                },
                IsCancelable = false,
                PostWorkCallBack = (argsDeleteHolding) =>
                {
                    if (argsDeleteHolding.Error != null)
                    {
                        LogError(string.Format("Error deleting holding managed solution: {0}", argsDeleteHolding.Error.ToString()));
                        MessageBox.Show(argsDeleteHolding.Error.Message.ToString(), "Error deleting holding managed solution", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var result = MessageBox.Show(string.Format("Solution {0} successfully raised", selectedSolution.OriginalSolutionName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExecuteMethod(LoadPublishers);
                        ExecuteMethod(LoadManagedSolutions);
                    }
                }
            });
        }

        private void DeleteOriginalSolution()
        {
            LogInfo(string.Format("Step 2 of 4: Deleting {0} managed solution", selectedSolution.OriginalSolutionName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 2 of 4: Deleting {0} managed solution", selectedSolution.OriginalSolutionName),
                Work = (worker, argsDeleteOriginal) =>
                {
                    SolutionRaisingStatus raisingStatus = SolutionValidator.GetSolutionRaisingStatus(Service, selectedSolution.OriginalSolutionName);
                    if (raisingStatus == SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionNotUninstalled)
                    {
                        bool success = SolutionManager.DeleteSolution(Service, selectedSolution.OriginalSolutionName);
                        if (!success)
                        {
                            argsDeleteOriginal.Result = false;
                            argsDeleteOriginal.Cancel = true;
                            throw new Exception("Delete of original solution failed or cancelled");
                        }
                        else
                        {
                            argsDeleteOriginal.Result = true;
                            argsDeleteOriginal.Cancel = false;
                        }
                    }
                    else
                    {
                        argsDeleteOriginal.Result = true;
                        argsDeleteOriginal.Cancel = false;
                    }
                },
                IsCancelable = false,
                PostWorkCallBack = (argsDeleteOriginal) =>
                {
                    if (argsDeleteOriginal.Error != null)
                    {
                        LogError(string.Format("Error deleting original managed solution: {0}", argsDeleteOriginal.Error.ToString()));
                        MessageBox.Show(argsDeleteOriginal.Error.Message.ToString(), "Error deleting original managed solution", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        ReinstallOriginalSolution();
                    }
                }
            });
        }

        private void ImportHoldingSolution()
        {
            LogInfo(string.Format("Step 1 of 4: Importing holding version of {0} managed solution", selectedSolution.OriginalSolutionName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 1 of 4: Importing holding version of {0} managed solution", selectedSolution.OriginalSolutionName),
                Work = (worker, argsImportHolding) =>
                {
                    SolutionRaisingStatus raisingStatus = SolutionValidator.GetSolutionRaisingStatus(Service, selectedSolution.OriginalSolutionName);
                    if (raisingStatus == SolutionRaisingStatus.NotStarted)
                    {
                        bool success = SolutionManager.ImportSolution(Service, fps.HoldingSolutionFilePath, selectedSolution.OriginalSolutionName + "_Holding");
                        if (!success)
                        {
                            argsImportHolding.Result = false;
                            argsImportHolding.Cancel = true;
                            throw new Exception("Holding solution import failed or cancelled");
                        }
                        else
                        {
                            argsImportHolding.Result = true;
                            argsImportHolding.Cancel = false;
                        }
                    }
                    else
                    {
                        argsImportHolding.Result = true;
                        argsImportHolding.Cancel = false;
                    }
                },
                IsCancelable = false,
                PostWorkCallBack = (argImportHolding) =>
                {
                    File.Delete(fps.HoldingSolutionFilePath);
                    if (argImportHolding.Error != null)
                    {
                        LogError(string.Format("Error importing holding managed solution: {0}", argImportHolding.Error.ToString()));
                        MessageBox.Show(argImportHolding.Error.Message.ToString(), "Error importing holding managed solution", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        DeleteOriginalSolution();
                    }
                }
            });
        }

        private void ReinstallOriginalSolution()
        {
            LogInfo(string.Format("Step 3 of 4: Importing {0} managed solution", selectedSolution.OriginalSolutionName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 3 of 4: Importing {0} managed solution", selectedSolution.OriginalSolutionName),
                Work = (worker, argsReinstallOriginal) =>
                {
                    SolutionRaisingStatus raisingStatus = SolutionValidator.GetSolutionRaisingStatus(Service, selectedSolution.OriginalSolutionName);
                    if (raisingStatus == SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionUninstalled)
                    {
                        bool success = SolutionManager.ImportSolution(Service, fps.OriginalSolutionFilePath, selectedSolution.OriginalSolutionName);
                        if (!success)
                        {
                            argsReinstallOriginal.Result = false;
                            argsReinstallOriginal.Cancel = true;
                            throw new Exception("Reinstall of original solution failed or cancelled");
                        }
                        else
                        {
                            argsReinstallOriginal.Result = true;
                            argsReinstallOriginal.Cancel = false;
                        }
                    }
                    else
                    {
                        argsReinstallOriginal.Result = true;
                        argsReinstallOriginal.Cancel = false;
                    }
                },
                IsCancelable = false,
                PostWorkCallBack = (argsReinstallOriginal) =>
                {
                    File.Delete(fps.OriginalSolutionFilePath);
                    if (argsReinstallOriginal.Error != null)
                    {
                        LogError(string.Format("Error installing original managed solution: {0}", argsReinstallOriginal.Error.ToString()));
                        MessageBox.Show(argsReinstallOriginal.Error.Message.ToString(), "Error installing original managed solution", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        DeleteHoldingSolution();
                    }
                }
            });
        }

        private void ProcessSelectedFile()
        {
            LogInfo(string.Format("Raising managed solution layer for {0} solution", selectedSolution.UniqueName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Raising managed solution layer for {0} solution", selectedSolution.UniqueName),
                Work = (worker, args) =>
                {
                    ValidateSelectedSolution(args);
                },
                IsCancelable = false,
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null || (bool)args.Result == false)
                    {
                        LogError(string.Format("Error raising managed solution layer: {0}", args.Error.ToString()));
                        MessageBox.Show(args.Error.Message.ToString(), "Solution Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var confirmationResult = MessageBox.Show("The raising of the selected managed solution may take a while to complete and may potentially disrupt normal usage of the target environment. It is recommended that this activity is performed as part of pre-agreed, scheduled and controlled application maintenance out of normal business hours. Are you sure you wish to continue with this process now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirmationResult == DialogResult.Yes)
                        {
                            ImportHoldingSolution();
                        }
                    }
                }
            });
        }

        private void LoadPublishers()
        {
            LogInfo("Retrieving publishers");
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving publishers",
                Work = (worker, args) =>
                {
                    EntityCollection results = PublisherManager.GetPublishers(Service);
                    args.Result = results;
                },
                IsCancelable = false,
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError(string.Format("Error retrieving publishers: {0}", args.Error.ToString()));
                        MessageBox.Show(args.Error.Message.ToString(), "Error retrieving publishers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var results = args.Result as EntityCollection;
                        LogInfo(string.Format("Successfully retrieved {0} publishers", results.Entities.Count));
                        tscbPublisher.Items.Clear();
                        if (results != null)
                        {
                            SortableBindingList<Publisher> publishers = new SortableBindingList<Publisher>();
                            foreach (Entity publisherRow in results.Entities)
                            {
                                Publisher publisher = new Publisher(publisherRow);
                                publishers.Add(publisher);

                                tscbPublisher.Items.Add(publisher);
                            }
                            tscbPublisher.ComboBox.DisplayMember = "FriendlyName";
                            tscbPublisher.ComboBox.ValueMember = "ID";
                        }
                    }
                }
            });
        }
        private void LoadManagedSolutions()
        {
            LogInfo("Retrieving managed solutions");
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving managed solutions",
                Work = (worker, args) =>
                {
                    EntityCollection results = SolutionManager.GetManagedSolutions(Service, tstbSearch.Text.Trim(), selectedPublisher);
                    args.Result = results;
                },
                IsCancelable = false,
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError(string.Format("Error retrieving managed solutions: {0}", args.Error.ToString()));
                        MessageBox.Show(args.Error.Message.ToString(), "Error retrieving managed solutions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var result = args.Result as EntityCollection;
                        LogInfo(string.Format("Successfully retrieved {0} managed solutions", result.Entities.Count));
                        if (result != null)
                        {
                            SortableBindingList<Solution> solutions = new SortableBindingList<Solution>();
                            foreach (Entity solution in result.Entities)
                            {
                                Solution solutionRow = new Solution(solution);
                                solutions.Add(solutionRow);
                            }
                            solutionsBindingSource.DataSource = solutions;
                            solutionsGridView.DataSource = solutionsBindingSource;
                            solutionsGridView.Columns[0].Visible = false;
                            solutionsGridView.Columns[3].Visible = false;
                            foreach (DataGridViewColumn column in solutionsGridView.Columns)
                            {
                                if (column.Index != 1)
                                {
                                    column.ReadOnly = true;
                                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                                }
                            }

                            tsbRaiseManagedSolution.Enabled = false;
                            tsbSearch.Enabled = (tstbSearch.Text.Trim().Length > 0);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManagedSolutionLayerRaiserControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            LogInfo("Closing tool Managed Solution Layer Raiser");
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                ShowInfoNotification("This tool performs solution imports and deletions in the target environment. Please ensure you use an account with system administrator privileges - ideally the account that you typically use for solution deployments", null);

                // Loads or creates the settings for the plugin
                if (!SettingsManager.Instance.TryLoad(typeof(XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.PluginControl), out mySettings))
                {
                    mySettings = new Settings();
                    mySettings.DefaultPathForTemporaryFiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    SettingsManager.Instance.Save(typeof(XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.PluginControl), mySettings);
                    LogWarning("Settings not found => a new settings file has been created!");
                }
                else
                {
                    LogInfo("Settings found and loaded");
                }
                ExecuteMethod(LoadPublishers);
                ExecuteMethod(LoadManagedSolutions);
            }
        }

        private void solutionsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                var isChecked = (bool)solutionsGridView.Rows[e.RowIndex].Cells[1].EditedFormattedValue;
                if (isChecked)
                {
                    foreach (DataGridViewRow row in solutionsGridView.Rows)
                    {
                        if (row.Index != e.RowIndex)
                        {
                            row.Cells[e.ColumnIndex].Value = !isChecked;
                        }
                    }
                }
                tsbRaiseManagedSolution.Enabled = isChecked;
            }
        }

        private void tsbRaiseManagedSolution_Click(object sender, EventArgs e)
        {
            int totalChecked = 0;
            foreach (DataGridViewRow row in solutionsGridView.Rows)
            {
                var isChecked = (bool)row.Cells[1].EditedFormattedValue;
                if (isChecked)
                {
                    selectedSolution = (Solution)row.DataBoundItem;
                    totalChecked++;
                }
            }
            if (totalChecked == 0)
            {
                MessageBox.Show("A solution needs to be selected first to raise", "No solution selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                selectSolutionFileDialog = new OpenFileDialog();
                selectSolutionFileDialog.InitialDirectory = ".";
                selectSolutionFileDialog.RestoreDirectory = true;
                selectSolutionFileDialog.Title = "Select managed solution file";
                selectSolutionFileDialog.Filter = "ZIP files (.zip)|*.zip";
                selectSolutionFileDialog.DefaultExt = "zip";
                selectSolutionFileDialog.CheckFileExists = true;
                selectSolutionFileDialog.CheckPathExists = true;
                selectSolutionFileDialog.Multiselect = false;
                var fileSelectionResult = selectSolutionFileDialog.ShowDialog();
                if (fileSelectionResult == DialogResult.OK)
                {
                    ExecuteMethod(ProcessSelectedFile);
                }
            }
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadManagedSolutions);
        }

        private void tstbSearch_TextChanged(object sender, EventArgs e)
        {
            var searchCriteriaEntered = (tstbSearch.Text.Trim().Length > 0);
            var publisherSelected = (selectedPublisher != null);
            tsbSearch.Enabled = searchCriteriaEntered || publisherSelected;
            if (!searchCriteriaEntered && !publisherSelected)
            {
                ExecuteMethod(LoadManagedSolutions);
            }
        }

        private void tscbPublisher_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPublisher = tscbPublisher.SelectedItem as Publisher;
            var searchCriteriaEntered = (tstbSearch.Text.Trim().Length > 0);
            var publisherSelected = (selectedPublisher != null);
            tsbSearch.Enabled = searchCriteriaEntered || publisherSelected;
            ExecuteMethod(LoadManagedSolutions);

        }
    }
}