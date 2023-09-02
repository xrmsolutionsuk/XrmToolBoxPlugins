﻿using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Organization;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic;
using XrmToolBox.Extensibility;
using Solution = XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic.Solution;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser
{
    public partial class PluginControl : PluginControlBase
    {
        private Settings mySettings;
        private BindingSource solutionsBindingSource = new BindingSource();
        OpenFileDialog selectSolutionFileDialog;
        BusinessLogic.Solution selectedSolution = null;

        public PluginControl()
        {
            InitializeComponent();
        }

        private void ManagedSolutionLayerRaiserControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This tool performs solution imports and deletions in the target environment. Please ensure you use an account with system administrator privileges - ideally the account that you typically use for solution deployments", null);

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

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
            ExecuteMethod(LoadManagedSolutions);
        }

        private void ValidateSelectedSolution(DoWorkEventArgs args)
        {
            string solutionXmlLocation = FileStructureGenerator.GenerateFileStructure(selectSolutionFileDialog.FileName);
            var solutionDoc = XDocument.Load(solutionXmlLocation);
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
                solutionDoc.Save(solutionXmlLocation);
                if (File.Exists("HoldingSolution.zip"))
                {
                    File.Delete("HoldingSolution.zip");
                }
                ZipFile.CreateFromDirectory("Holding Solution", "HoldingSolution.zip");
                args.Result = true;
            }
        }

        private void DeleteHoldingSolution()
        {
            LogInfo(string.Format("Step 4 of 4: Deleting {0} holding managed solution", selectedSolution.UniqueName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 4 of 4: Deleting {0} holding managed solution", selectedSolution.UniqueName),
                Work = (worker, argsDeleteHolding) =>
                {
                    bool success = SolutionManager.DeleteSolution(Service, selectedSolution.UniqueName + "_Holding");
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
                },
                PostWorkCallBack = (argsDeleteHolding) =>
                {
                    if (argsDeleteHolding.Error != null)
                    {
                        LogError(string.Format("Error deleting holding managed solution: {0}", argsDeleteHolding.Error.ToString()));
                        MessageBox.Show(argsDeleteHolding.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
        }

        private void DeleteOriginalSolution()
        {
            LogInfo(string.Format("Step 2 of 4: Deleting {0} managed solution", selectedSolution.UniqueName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 2 of 4: Deleting {0} managed solution", selectedSolution.UniqueName),
                Work = (worker, argsDeleteOriginal) =>
                {
                    bool success = SolutionManager.DeleteSolution(Service, selectedSolution.UniqueName);
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
                },
                PostWorkCallBack = (argsDeleteOriginal) =>
                {
                    if (argsDeleteOriginal.Error != null)
                    {
                        LogError(string.Format("Error deleting original managed solution: {0}", argsDeleteOriginal.Error.ToString()));
                        MessageBox.Show(argsDeleteOriginal.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LogInfo(string.Format("Step 1 of 4: Importing holding version of {0} managed solution", selectedSolution.UniqueName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 1 of 4: Importing holding version of {0} managed solution", selectedSolution.UniqueName),
                Work = (worker, argsImportHolding) =>
                {
                    bool success = SolutionManager.ImportSolution(Service, "HoldingSolution.zip");
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
                },
                PostWorkCallBack = (argImportHolding) =>
                {
                    if (argImportHolding.Error != null)
                    {
                        LogError(string.Format("Error importing holding managed solution: {0}", argImportHolding.Error.ToString()));
                        MessageBox.Show(argImportHolding.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LogInfo(string.Format("Step 3 of 4: Importing {0} managed solution", selectedSolution.UniqueName));
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Step 3 of 4: Importing {0} managed solution", selectedSolution.UniqueName),
                Work = (worker, argsReinstallOriginal) =>
                {
                    bool success = SolutionManager.ImportSolution(Service, "OriginalSolution.zip");
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
                },
                PostWorkCallBack = (argsReinstallOriginal) =>
                {
                    if (argsReinstallOriginal.Error != null)
                    {
                        LogError(string.Format("Error installing original managed solution: {0}", argsReinstallOriginal.Error.ToString()));
                        MessageBox.Show(argsReinstallOriginal.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 120, 0);
            WorkAsync(new WorkAsyncInfo
            {
                Message = string.Format("Raising managed solution layer for {0} solution", selectedSolution.UniqueName),
                Work = (worker, args) =>
                {
                    ValidateSelectedSolution(args);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError(string.Format("Error raising managed solution layer: {0}", args.Error.ToString()));
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        ImportHoldingSolution();
                    }
                }
            });
        }
        private void LoadManagedSolutions()
        {
            LogInfo("Retrieving managed solutions");
            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 120, 0);
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving managed solutions",
                Work = (worker, args) =>
                {
                    EntityCollection results = QueryManager.GetManagedSolutions(tstbSearch.Text.Trim(), Service);
                    args.Result = results;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError(string.Format("Error retrieving managed solution: {0}", args.Error.ToString()));
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);

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
                MessageBox.Show("A solution needs to be selected first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void tstbSearch_TextChanged(object sender, EventArgs e)
        {
            tsbSearch.Enabled = (tstbSearch.Text.Trim().Length > 0);
            if (tstbSearch.Text.Trim().Length == 0)
            {
                ExecuteMethod(LoadManagedSolutions);
            }
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadManagedSolutions);
        }
    }
}