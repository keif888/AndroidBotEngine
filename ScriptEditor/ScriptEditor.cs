// <copyright file="ScriptEditor.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

#region Usings
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using BotEngineClient;
using SharpAdbClient;
using static BotEngineClient.BotEngine;
using System.Text.Json.Nodes;
using System.Threading;
#endregion

namespace ScriptEditor
{
    public partial class ScriptEditor : Form
    {
        #region Privates
        private static BOTConfig gameConfig;
        private static BOTDeviceConfig deviceConfig;
        private static BOTListConfig listConfig;
        private bool ChangePending;
        private bool UnsavedChanges;
        private AdbServer server;
        private List<string> devicesList;
        private JsonHelper.ConfigFileType loadedFileType;
        private TreeNode ActiveTreeNode;
        private static FileSystemWatcher fileWatcher;
        private static string JsonFileName;
        private static bool ReloadTreeViewRequired;
        #endregion

        #region Constructor
        /// <summary>
        /// Class constructor to default all privates.
        /// </summary>
        public ScriptEditor()
        {
            InitializeComponent();
            gameConfig = new BOTConfig();
            deviceConfig = new BOTDeviceConfig();
            listConfig = new BOTListConfig();
            // Reset all the Group Boxes.
            ResetGroupBox(gbClick);
            ResetGroupBox(gbDrag);
            ResetGroupBox(gbImageName);
            ResetGroupBox(gbImageNameAndWait);
            ResetGroupBox(gbImageNames);
            ResetGroupBox(gbLoopCoordinate);
            ResetGroupBox(gbSleep);
            ResetGroupBox(gbAction);
            ResetGroupBox(gbFindText);
            ResetGroupBox(gbWFNC);
            ResetGroupBox(gbAppControl);
            ResetGroupBox(gbPickAction);
            ResetGroupBox(gbAppName);
            ResetGroupBox(gbActionOverride);
            ResetGroupBox(gbImageArea);
            ResetGroupBox(gbList);
            ResetGroupBox(gbLoops);

            this.Size = new Size(800, 485);
            splitContainer1.SplitterDistance = 320;
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.Error;
            ActiveTreeNode = null;
            btnUpdate.Enabled = false;
            ReloadTreeViewRequired = false;
        }

        #endregion

        // ToDo: Add GrabWindow, that isn't Modal, so you can get Coords etc from it
        // ToDo: Add Defer Command which sets a scheduled task to retry n minutes in the future.This should incorporate restart from where it was
        // ToDo: Add Insert Above/Below for FindString
        // ToDo: Fix ComboBoxes when Delete/Rename items

        #region File Menu
        /// <summary>
        /// Menu method to allow the opening of a json file and then loading it into the tvBotData.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                LoadConfigFile(fileName);
            }
        }

        /// <summary>
        /// Gets the file name and type for the new json file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTypeSelect fileTypeSelect = new FileTypeSelect();

            if (fileTypeSelect.ShowDialog() == DialogResult.OK)
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    loadedFileType = fileTypeSelect.ConfigFileType;
                    JsonFileName = saveFileDialog1.FileName;
                    tvBotData.SuspendLayout();
                    tvBotData.Nodes.Clear();
                    cbImageNameNoWait.Items.Clear();
                    cbImageNameWithWait.Items.Clear();
                    cbImageNamesForList.Items.Clear();
                    cbImageAreasImage.Items.Clear();
                    cbPickActionAction.Items.Clear();
                    cbActionBefore.Items.Clear();
                    cbActionAfter.Items.Clear();
                    ResetEditFormItems();

                    switch (loadedFileType)
                    {
                        case JsonHelper.ConfigFileType.GameConfig:
                            TreeNode findStringsNode = tvBotData.Nodes.Add("FindStrings");
                            findStringsNode.Name = "FindStrings";
                            TreeNode systemActionsNode = tvBotData.Nodes.Add("SystemActions");
                            systemActionsNode.Name = "SystemActions";
                            TreeNode actionsNode = tvBotData.Nodes.Add("Actions");
                            actionsNode.Name = "Actions";
                            break;
                        case JsonHelper.ConfigFileType.ListConfig:
                            TreeNode coordinatesNode = tvBotData.Nodes.Add("Coordinates");
                            coordinatesNode.Name = "Coordinates";
                            break;
                        case JsonHelper.ConfigFileType.DeviceConfig:
                            TreeNode lastActionNode = tvBotData.Nodes.Add("LastActionTaken");
                            lastActionNode.Name = "LastActionTaken";
                            break;
                        default:
                            break;
                    }


                    tvBotData.ResumeLayout();
                    ChangePending = false;
                    UnsavedChanges = false;
                    saveToolStripMenuItem.Enabled = false;
                    closeToolStripMenuItem.Enabled = true;
                }
        }


        /// <summary>
        /// Saves the data in tvBotData into the json file that it came from.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool fileWatcherStatus = false;
            if (fileWatcher != null)
                fileWatcherStatus = fileWatcher.EnableRaisingEvents;
            fileWatcher.EnableRaisingEvents = false;
            switch (loadedFileType)
            {
                case JsonHelper.ConfigFileType.GameConfig:
                    SaveGameConfig();
                    break;
                case JsonHelper.ConfigFileType.ListConfig:
                    SaveListConfig();
                    break;
                case JsonHelper.ConfigFileType.DeviceConfig:
                    SaveDeviceConfig();
                    break;
                default:
                    break;
            }
            UnsavedChanges = false;
            if (fileWatcher != null)
                fileWatcher.EnableRaisingEvents = fileWatcherStatus;
        }

        /// <summary>
        /// Closes the open file, and clears the tree view, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ChangePending || UnsavedChanges)
            {
                if (MessageBox.Show("There are Unsaved or Pending Changes, Close File?", "Close File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }
            UnsavedChanges = false;
            ChangePending = false;
            JsonFileName = string.Empty;
            fileWatcher.EnableRaisingEvents = false;
            tvBotData.SuspendLayout();
            tvBotData.Nodes.Clear();
            tvBotData.ResumeLayout();
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.Error;
            ActiveTreeNode = null;
            btnUpdate.Enabled = false;
            ReloadTreeViewRequired = false;
            ResetEditFormItems();
            closeToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Exit command from the Menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Edit Menu

        /// <summary>
        /// Add a new FindString node into the tvBotData tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFindStringtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEditFormItems();
            if (tvBotData.SelectedNode.Nodes.ContainsKey("New FindString"))
            {
                TreeNode selectedNode = tvBotData.SelectedNode.Nodes["New FindString"];
                tvBotData.SelectedNode = selectedNode;
                MessageBox.Show("Rename this FindString to allow new find strings to be added");
            }
            else
            {
                FindString newFindString = new FindString();

                TreeNode newNode = new TreeNode {
                    Text = "New FindString",
                    Name = "New FindString",
                    Tag = newFindString
                };

                tvBotData.SelectedNode.Nodes.Add(newNode);
                tvBotData.SelectedNode = newNode;
            }
        }

        /// <summary>
        /// Add a new Action into the tvBotData tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEditFormItems();
            if (tvBotData.SelectedNode.Nodes.ContainsKey("New Action"))
            {
                TreeNode selectedNode = tvBotData.SelectedNode.Nodes["New Action"];
                tvBotData.SelectedNode = selectedNode;
                MessageBox.Show("Rename this Action to allow new actions to be added");
            }
            else
            {
                BotEngineClient.Action newAction = new BotEngineClient.Action();
                if (tvBotData.SelectedNode.Name == "Actions")
                    newAction.ActionType = ValidActionType.Scheduled.ToString();
                else
                    newAction.ActionType = ValidActionType.System.ToString();

                TreeNode newNode = new TreeNode {
                    Text = "New Action",
                    Name = "New Action",
                    Tag = newAction
                };

                tvBotData.SelectedNode.Nodes.Add(newNode);
                tvBotData.SelectedNode = newNode;
            }
        }

        /// <summary>
        /// Add a new Coordinates node into the tvBotData tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCoordinatestoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEditFormItems();
            if (tvBotData.SelectedNode.Nodes.ContainsKey("New Coordinates"))
            {
                TreeNode selectedNode = tvBotData.SelectedNode.Nodes["New Coordinates"];
                tvBotData.SelectedNode = selectedNode;
                MessageBox.Show("Rename this Coordinates to allow new coordinates to be added");
            }
            else if (tvBotData.SelectedNode.Tag is List<XYCoords>)
            {
                TreeNode treeNode = new TreeNode {
                    Text = "(0,0)",
                    Name = "(0,0)",
                    Tag = new XYCoords(0, 0)
                };
                tvBotData.SelectedNode.Nodes.Add(treeNode);
                tvBotData.SelectedNode = treeNode;
            }
            else
            {
                List<XYCoords> newCoordinates = new List<XYCoords>();

                TreeNode newNode = new TreeNode {
                    Text = "New Coordinates",
                    Name = "New Coordinates",
                    Tag = newCoordinates
                };

                tvBotData.SelectedNode.Nodes.Add(newNode);
                tvBotData.SelectedNode = newNode;
            }
        }

        /// <summary>
        /// Add a command to a selected tvBotData item that is allowed to have them added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            if (currentNode.Tag != null)
            {
                if (currentNode.Tag is Command || currentNode.Tag is BotEngineClient.Action)
                {
                    CommandSelect commandSelect = new CommandSelect();
                    if (commandSelect.ShowDialog() == DialogResult.OK)
                    {
                        string commandId = commandSelect.SelectedCommand.ToString();
                        Command newCommand = new Command(commandSelect.SelectedCommand);
                        string childText = GetCommandIdDisplayText(newCommand);
                        TreeNode newNode = new TreeNode {
                            Tag = newCommand,
                            Name = commandId,
                            Text = childText
                        };
                        currentNode.Nodes.Add(newNode);
                        tvBotData.SelectedNode = newNode;
                        UnsavedChanges = true;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a new node, before the current node into the tvBotData tree, and makes it active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            int currentNodeIndex = tvBotData.SelectedNode.Index;
            TreeNode parent = tvBotData.SelectedNode.Parent;
            if (currentNode.Tag != null)
            {
                if (currentNode.Tag is Command)
                {
                    CommandSelect commandSelect = new CommandSelect();
                    if (commandSelect.ShowDialog() == DialogResult.OK)
                    {
                        string commandId = commandSelect.SelectedCommand.ToString();
                        Command newCommand = new Command(commandSelect.SelectedCommand);
                        string childText = GetCommandIdDisplayText(newCommand);
                        TreeNode newNode = new TreeNode {
                            Tag = newCommand,
                            Name = commandId,
                            Text = childText
                        };
                        parent.Nodes.Insert(currentNodeIndex, newNode);
                        tvBotData.SelectedNode = newNode;
                        UnsavedChanges = true;
                    }
                }
                else if (currentNode.Tag is XYCoords)
                {
                    XYCoords newCoords = new XYCoords(0, 0);
                    TreeNode newNode = new TreeNode {
                        Name = "(0,0)",
                        Tag = newCoords,
                        Text = "(0,0)"
                    };
                    parent.Nodes.Insert(currentNodeIndex, newNode);
                    UnsavedChanges = true;
                    tvBotData.SelectedNode = newNode;
                }
                else if (currentNode.Tag is List<XYCoords>)
                {
                    XYCoords newCoords = new XYCoords(0, 0);
                    TreeNode newNode = new TreeNode {
                        Name = "(0,0)",
                        Tag = newCoords,
                        Text = "(0,0)"
                    };
                    currentNode.Nodes.Add(newNode);
                    UnsavedChanges = true;
                    tvBotData.SelectedNode = newNode;
                }
            }
        }

        /// <summary>
        /// Inserts a new node, after the current node into the tvBotData tree, and makes it active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BelowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            int currentNodeIndex = tvBotData.SelectedNode.Index;
            TreeNode parent = tvBotData.SelectedNode.Parent;
            if (currentNode.Tag != null)
            {
                if (currentNode.Tag is Command)
                {
                    CommandSelect commandSelect = new CommandSelect();
                    if (commandSelect.ShowDialog() == DialogResult.OK)
                    {
                        string commandId = commandSelect.SelectedCommand.ToString();
                        Command newCommand = new Command(commandSelect.SelectedCommand);
                        string childText = GetCommandIdDisplayText(newCommand);
                        TreeNode newNode = new TreeNode {
                            Tag = newCommand,
                            Name = commandId,
                            Text = childText
                        };
                        parent.Nodes.Insert(currentNodeIndex + 1, newNode);
                        tvBotData.SelectedNode = newNode;
                        UnsavedChanges = true;
                    }
                }
                else if (currentNode.Tag is BotEngineClient.Action)
                {
                    CommandSelect commandSelect = new CommandSelect();
                    if (commandSelect.ShowDialog() == DialogResult.OK)
                    {
                        string commandId = commandSelect.SelectedCommand.ToString();
                        Command newCommand = new Command(commandSelect.SelectedCommand);
                        string childText = GetCommandIdDisplayText(newCommand);
                        TreeNode newNode = new TreeNode {
                            Tag = newCommand,
                            Name = commandId,
                            Text = childText
                        };
                        currentNode.Nodes.Add(newNode);
                        tvBotData.SelectedNode = newNode;
                        UnsavedChanges = true;
                    }
                }
                else if (currentNode.Tag is XYCoords)
                {
                    XYCoords newCoords = new XYCoords(0, 0);
                    TreeNode newNode = new TreeNode {
                        Name = "(0,0)",
                        Tag = newCoords,
                        Text = "(0,0)"
                    };
                    parent.Nodes.Insert(currentNodeIndex + 1, newNode);
                    tvBotData.SelectedNode = newNode;
                    UnsavedChanges = true;
                }
                else if (currentNode.Tag is List<XYCoords>)
                {
                    XYCoords newCoords = new XYCoords(0, 0);
                    TreeNode newNode = new TreeNode {
                        Name = "(0,0)",
                        Tag = newCoords,
                        Text = "(0,0)"
                    };
                    currentNode.Nodes.Add(newNode);
                    UnsavedChanges = true;
                    tvBotData.SelectedNode = newNode;
                }
            }
        }

        /// <summary>
        /// Moves the selected tvBotNode node up one spot, if it can be moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            if (currentNode != null && currentNode.Parent != null)
            {
                TreeNode parentNode = currentNode.Parent;
                int treeIndex = parentNode.Nodes.IndexOf(currentNode);
                if (treeIndex > 0)
                {
                    parentNode.Nodes.RemoveAt(treeIndex);
                    parentNode.Nodes.Insert(treeIndex - 1, currentNode);
                    tvBotData.SelectedNode = currentNode;
                    UnsavedChanges = true;
                }
            }
        }

        /// <summary>
        /// Moves the selected tvBotNode node down one spot, if it can be moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            if (currentNode != null && currentNode.Parent != null)
            {
                TreeNode parentNode = currentNode.Parent;
                int treeIndex = parentNode.Nodes.IndexOf(currentNode);
                if (treeIndex < parentNode.Nodes.Count - 1)
                {
                    parentNode.Nodes.RemoveAt(treeIndex);
                    parentNode.Nodes.Insert(treeIndex + 1, currentNode);
                    tvBotData.SelectedNode = currentNode;
                    UnsavedChanges = true;
                }
            }
        }

        /// <summary>
        /// Delete the seleted node from tvBotData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            // ToDo: Add warning if deleting FindText or Action.
            // ToDo: Update the combo boxes for FindText and Actions.
            if (currentNode != null)
            {
                tvBotData.Nodes.Remove(currentNode);
                UnsavedChanges = true;
                ChangePending = false;
            }
        }

        #endregion

        #region Text Menu
        /// <summary>
        /// Shows the dialog to create FindString's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextEdit fte = new FindTextEdit();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                if ((!Clipboard.ContainsText()) || (Clipboard.ContainsText() && !Clipboard.GetText().StartsWith("{")))
                {
                    string searchText = fte.SearchText;
                    Rectangle searchArea = fte.SearchRectangle;
                    string clipboard = string.Format("{{\"FindString\":\"{0}\", \"searchArea\":{{\"X\":{1}, \"Y\":{2}, \"width\":{3}, \"height\":{4}}}}}", searchText, searchArea.X, searchArea.Y, searchArea.Width, searchArea.Height);
                    Clipboard.SetText(clipboard);
                }
            }
        }

        /// <summary>
        /// Shows the dialog to enable testing if a FindString can be detected via ADB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextValidate findTextValidate = new FindTextValidate();
            if (!RefreshDeviceList())
            {
                return;
            }
            RefreshGameConfig();
            findTextValidate.SetupFindTextValidate(gameConfig, devicesList);
            findTextValidate.ShowDialog();
        }

        #endregion

        #region Test Menu
        /// <summary>
        /// Will implement ability to execute actions and capture logging from Editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string listConfigName;
            string saveTitle = openFileDialog1.Title;

            openFileDialog1.Title = "Select the List Config File to use for testing";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialog1.Title = saveTitle;
                listConfigName = openFileDialog1.FileName;
                JsonHelper jsonHelper = new JsonHelper();
                if (jsonHelper.GetFileType(listConfigName) != JsonHelper.ConfigFileType.ListConfig)
                {
                    MessageBox.Show("File selected wasn't a List Config File.", "Select correct file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string jsonString = File.ReadAllText(listConfigName);
                    try
                    {
                        listConfig = JsonSerializer.Deserialize<BOTListConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip, PropertyNameCaseInsensitive = true })!;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Unable to parse file.\r\n{0}", ex.Message));
                        return;
                    }
                    TestAction testAction = new TestAction();
                    if (!RefreshDeviceList())
                    { 
                        return;
                    }
                    testAction.SetupTestActionForm(gameConfig, listConfig, devicesList);
                    testAction.ShowDialog();
                }
            }
            else
            {
                openFileDialog1.Title = saveTitle;
            }

        }
        #endregion

        #region Help Menu
        /// <summary>
        /// Show the About box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAboutBox about = new HelpAboutBox();
            about.ShowDialog();
        }

        #endregion

        #region Load Files
        /// <summary>
        /// Loads the selected file into the TreeView.
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadConfigFile(string fileName)
        {
            ReloadTreeViewRequired = false;
            JsonHelper jsonHelper = new JsonHelper();
            switch (jsonHelper.GetFileType(fileName))
            {
                case JsonHelper.ConfigFileType.DeviceConfig:
                    if (jsonHelper.ValidateDeviceConfigStructure(fileName))
                    {
                        LoadDeviceConfigFile(fileName);
                        setupToolStripMenuItem.Enabled = false;
                        validateToolStripMenuItem.Enabled = false;
                        testToolStripMenuItem.Enabled = false;
                    }
                    break;
                case JsonHelper.ConfigFileType.GameConfig:
                    if (jsonHelper.ValidateGameConfigStructure(fileName))
                    {
                        LoadGameConfigFile(fileName);
                        setupToolStripMenuItem.Enabled = true;
                        validateToolStripMenuItem.Enabled = true;
                        testToolStripMenuItem.Enabled = true;
                    }
                    break;
                case JsonHelper.ConfigFileType.ListConfig:
                    if (jsonHelper.ValidateListConfigStructure(fileName))
                    {
                        LoadListConfigFile(fileName);
                        setupToolStripMenuItem.Enabled = false;
                        validateToolStripMenuItem.Enabled = false;
                        testToolStripMenuItem.Enabled = false;
                    }
                    break;
                default:
                    break;
            }
            if (jsonHelper.Errors.Count > 0)
            {
                JsonErrors jsonErrors = new JsonErrors();
                StringBuilder errorString = new StringBuilder();
                errorString.AppendLine(string.Format("The following errors were encountered when attempting to load the file {0}", fileName));
                errorString.AppendLine(new string('-', 48));
                errorString.AppendLine("| The file has not been loaded into the editor. |");
                errorString.AppendLine(new string('-', 48));
                errorString.AppendLine("");
                foreach (string item in jsonHelper.Errors)
                {
                    errorString.AppendLine(item);
                }
                jsonErrors.setText(errorString);
                jsonErrors.ShowDialog();
            }
            else
            {
                JsonFileName = fileName;
                EnableFileWatcher(fileName);
                ResetEditFormItems();
                tvBotData.Nodes[0].EnsureVisible(); // Scroll to Top
                closeToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Loads a game config file into tvBotData and it's Tags.
        /// </summary>
        /// <param name="fileName">The file name and path of a game config file</param>
        private void LoadGameConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                gameConfig = JsonSerializer.Deserialize<BOTConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip, PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to parse file.\r\n{0}", ex.Message));
                return;
            }
            tvBotData.SuspendLayout();
            tvBotData.Nodes.Clear();
            cbImageNameNoWait.Items.Clear();
            cbImageNameWithWait.Items.Clear();
            cbImageNamesForList.Items.Clear();
            cbPickActionAction.Items.Clear();
            cbActionAfter.Items.Clear();
            cbActionBefore.Items.Clear();
            cbImageAreasImage.Items.Clear();
            TreeNode findStringsNode = tvBotData.Nodes.Add("FindStrings");
            findStringsNode.Name = "FindStrings";
            foreach (KeyValuePair<string, BotEngineClient.FindString> item in gameConfig.FindStrings)
            {
                TreeNode treeNode = new TreeNode
                {
                    Name = item.Key,
                    Tag = item.Value,
                    Text = item.Key
                };
                findStringsNode.Nodes.Add(treeNode);
                cbImageNameNoWait.Items.Add(item.Key);
                cbImageNameWithWait.Items.Add(item.Key);
                cbImageNamesForList.Items.Add(item.Key);
                cbImageAreasImage.Items.Add(item.Key);
            }
            tvBotData.Sort();  // Do not do this again after loading the findStrings, as it will stuff every thing up.
            tvBotData.Sorted = false;

            findStringsNode.Expand();
            TreeNode systemActionsNode = tvBotData.Nodes.Add("SystemActions");
            systemActionsNode.Name = "SystemActions";
            foreach (KeyValuePair<string, BotEngineClient.Action> item in gameConfig.SystemActions)
            {
                LoadActionTreeNode(systemActionsNode, item);
                cbPickActionAction.Items.Add(item.Key);
                cbActionAfter.Items.Add(item.Key);
                cbActionBefore.Items.Add(item.Key);
            }
            systemActionsNode.Expand();
            TreeNode actionsNode = tvBotData.Nodes.Add("Actions");
            actionsNode.Name = "Actions";
            foreach (KeyValuePair<string, BotEngineClient.Action> item in gameConfig.Actions)
            {
                LoadActionTreeNode(actionsNode, item);
                cbPickActionAction.Items.Add(item.Key);
                cbActionAfter.Items.Add(item.Key);
                cbActionBefore.Items.Add(item.Key);
            }
            actionsNode.Expand();

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.GameConfig;
        }

        /// <summary>
        /// Loads a device config file into tvBotData and it's Tags.
        /// </summary>
        /// <param name="fileName">The file name and path of a device config file</param>
        private void LoadDeviceConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                deviceConfig = JsonSerializer.Deserialize<BOTDeviceConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip, PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to parse file.\r\n{0}", ex.Message));
                return;
            }
            tvBotData.SuspendLayout();
            tvBotData.Nodes.Clear();
            cbImageNameNoWait.Items.Clear();
            cbImageNameWithWait.Items.Clear();
            cbImageNamesForList.Items.Clear();
            cbPickActionAction.Items.Clear();
            cbActionAfter.Items.Clear();
            cbActionBefore.Items.Clear();
            TreeNode lastActionNode = tvBotData.Nodes.Add("LastActionTaken");
            lastActionNode.Name = "LastActionTaken";
            foreach (KeyValuePair<string, BotEngineClient.ActionActivity> item in deviceConfig.LastActionTaken)
            {
                TreeNode treeNode = new TreeNode
                {
                    Name = item.Key,
                    Tag = item.Value,
                    Text = string.Format("{0} - {1}", item.Key, item.Value.ActionEnabled ? "Enabled" : "Disabled") 
                };
                lastActionNode.Nodes.Add(treeNode);
            }
            lastActionNode.Expand();

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.DeviceConfig;
        }

        /// <summary>
        /// Loads a game config file into tvBotData and it's Tags.
        /// </summary>
        /// <param name="fileName">The file name and path of a game config file</param>
        private void LoadListConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                listConfig = JsonSerializer.Deserialize<BOTListConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip, PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to parse file.\r\n{0}", ex.Message));
                return;
            }
            tvBotData.SuspendLayout();
            tvBotData.Nodes.Clear();
            cbImageNameNoWait.Items.Clear();
            cbImageNameWithWait.Items.Clear();
            cbImageNamesForList.Items.Clear();
            cbPickActionAction.Items.Clear();
            cbActionAfter.Items.Clear();
            cbActionBefore.Items.Clear();

            TreeNode coordinatesNode = tvBotData.Nodes.Add("Coordinates");
            coordinatesNode.Name = "Coordinates";
            foreach (KeyValuePair<string, List<XYCoords>> item in listConfig.Coordinates)
            {
                TreeNode treeNode = new TreeNode
                {
                    Name = item.Key,
                    Tag = item.Value,
                    Text = item.Key
                };

                foreach (XYCoords coord in item.Value)
                {
                    TreeNode child = new TreeNode
                    {
                        Name = string.Format("({0},{1})", coord.X, coord.Y),
                        Tag = coord,
                        Text = string.Format("({0},{1})", coord.X, coord.Y)
                    };
                    treeNode.Nodes.Add(child);
                }
                coordinatesNode.Nodes.Add(treeNode);
            }
            coordinatesNode.Expand();

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.ListConfig;
        }

        /// <summary>
        /// Add new children to nodes in the tvBotData with optional Command information
        /// </summary>
        /// <param name="parent">The Node in tvBotData which this child is to added to</param>
        /// <param name="item">The information on what to add</param>
        private void LoadActionTreeNode(TreeNode parent, KeyValuePair<string, BotEngineClient.Action> item)
        {
            TreeNode child = new TreeNode
            {
                Name = item.Key
            };
            if (item.Value.Commands != null)
            {
                LoadActionTreeNode(child, item.Value.Commands);

            }
            child.Tag = item.Value;
            child.Text = item.Key;
            parent.Nodes.Add(child);
        }

        /// <summary>
        /// Add new children to nodes in the tvBotData with Command information
        /// </summary>
        /// <param name="parent">The Node in tvBotData which this child is to added to</param>
        /// <param name="commands">The commands to load as new children of the parent</param>
        private void LoadActionTreeNode(TreeNode parent, List<BotEngineClient.Command> commands)
        {
            foreach (BotEngineClient.Command command in commands)
            {

                string childText = GetCommandIdDisplayText(command);
                // Create a copy of the command, to put in the Tag, as we don't want the Commands.
                // They will be recomposed from the TreeNodes.
                BotEngineClient.Command commandCopy = command.DeepCopy();
                if (commandCopy.Commands != null)
                {
                    commandCopy.Commands.Clear();
                    commandCopy.Commands = null;
                }
                TreeNode child = new TreeNode
                {
                    Name = command.CommandNumber.ToString(),  //ToDo: Check if this could be childText
                    Tag = commandCopy,
                    Text = childText
                };
                if (command.Commands != null)
                {
                    LoadActionTreeNode(child, command.Commands);
                }
                parent.Nodes.Add(child);
            }
        }
        #endregion

        #region Save Files


        /// <summary>
        /// Saves a List Config gile from the content saved within the tvBotData tree view and it's Tags
        /// </summary>
        private void SaveListConfig()
        {
            BOTListConfig listConfig = new BOTListConfig
            {
                FileId = loadedFileType.ToString(),
                Coordinates = new Dictionary<string, List<XYCoords>>()
            };
            foreach (TreeNode parent in tvBotData.Nodes)
            {
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "Coordinates")
                {
                    foreach (TreeNode coordList in parent.Nodes)
                    {
                        List<XYCoords> coords = new List<XYCoords>();
                        foreach (TreeNode child in coordList.Nodes)
                        {
                            coords.Add((XYCoords)child.Tag);
                        }
                        listConfig.Coordinates.Add(coordList.Name, coords);
                    }
                }
            }
            try
            {
                if (File.Exists(JsonFileName))
                {
                    if (File.Exists(JsonFileName + ".bak"))
                    {
                        File.Delete(JsonFileName + ".bak");
                    }
                    File.Copy(JsonFileName, JsonFileName + ".bak");
                }
                string jsonData = JsonSerializer.Serialize<BOTListConfig>(listConfig, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                File.WriteAllText(JsonFileName, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to save file with {0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves a Game Config file from the content saved within the tvBotData tree view and it's Tags
        /// </summary>
        private void SaveGameConfig()
        {
            RefreshGameConfig();
            try
            {
                if (File.Exists(JsonFileName))
                {
                    if (File.Exists(JsonFileName + ".bak"))
                    {
                        File.Delete(JsonFileName + ".bak");
                    }
                    File.Copy(JsonFileName, JsonFileName + ".bak");
                }
                string jsonData = JsonSerializer.Serialize<BOTConfig>(gameConfig, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                File.WriteAllText(JsonFileName, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to save file with {0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Takes the data from the tvBotData, and loads it into the appropriate objects
        /// </summary>
        /// <param name="commands">The list that is being added to</param>
        /// <param name="parent">The parent Tree Node to get all the children from</param>
        /// <param name="commandNumber">The current command number</param>
        private void LoadCommandList(List<Command> commands, TreeNode parent, ref int commandNumber)
        {
            foreach (TreeNode childNode in parent.Nodes)
            {
                Command childCommand = (Command)childNode.Tag;
                Command newCommand = new Command(childCommand.CommandId)
                {
                    ActionName = childCommand.ActionName,
                    Areas = childCommand.Areas,
                    ChangeDetectArea = childCommand.ChangeDetectArea,
                    ChangeDetectDifference = childCommand.ChangeDetectDifference,
                    CommandNumber = commandNumber
                };
                commandNumber += 10;
                if (childNode.Nodes.Count != 0)
                {
                    List<Command> childCommands = new List<Command>();
                    LoadCommandList(childCommands, childNode, ref commandNumber);
                    newCommand.Commands = childCommands;
                }
                newCommand.Coordinates = childCommand.Coordinates;
                newCommand.Delay = childCommand.Delay;
                newCommand.IgnoreMissing = childCommand.IgnoreMissing;
                newCommand.ImageName = childCommand.ImageName;
                newCommand.ImageNames = childCommand.ImageNames;
                newCommand.Location = childCommand.Location;
                newCommand.Swipe = childCommand.Swipe;
                newCommand.TimeOut = childCommand.TimeOut;
                newCommand.Value = childCommand.Value;
                newCommand.OverrideId = childCommand.OverrideId;
                commands.Add(newCommand);
            }
        }


        /// <summary>
        /// Saves a Device Config file from the content saved within the tvBotData tree view and it's Tags
        /// </summary>
        private void SaveDeviceConfig()
        {
            BOTDeviceConfig deviceConfig = new BOTDeviceConfig
            {
                FileId = loadedFileType.ToString(),
                LastActionTaken = new Dictionary<string, ActionActivity>()
            };
            foreach (TreeNode parent in tvBotData.Nodes)
            {
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "LastActionTaken")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        deviceConfig.LastActionTaken.Add(child.Name, (ActionActivity)child.Tag);
                    }
                }
            }
            try
            {
                if (File.Exists(JsonFileName))
                {
                    if (File.Exists(JsonFileName + ".bak"))
                    {
                        File.Delete(JsonFileName + ".bak");
                    }
                    File.Copy(JsonFileName, JsonFileName + ".bak");
                }
                string jsonData = JsonSerializer.Serialize<BOTDeviceConfig>(deviceConfig, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                File.WriteAllText(JsonFileName, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to save file with {0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region TvBotData Events
        /// <summary>
        /// After selecting a node in the tvBotData, show the appropriate edit GroupBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvBotData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ReloadTreeViewRequired)
            {
                LoadConfigFile(JsonFileName);
                return;
            }
            ResetEditFormItems();

            if (e.Node != null)
                ActiveTreeNode = e.Node;
            if (e.Node.Parent != null)
                if ((e.Node.Parent.Nodes.Count > 1) && !(e.Node.Tag is BotEngineClient.Action))
                {
                    upToolStripMenuItem.Enabled = true;
                    downToolStripMenuItem.Enabled = true;
                }
            if (e.Node.Tag != null)
            {
                deleteToolStripMenuItem.Enabled = true;
            }
            if (e.Node.Tag == null)
            {
                switch (e.Node.Name)
                {
                    case "SystemActions":
                    case "Actions":
                        addActionToolStripMenuItem.Enabled = true;
                        break;
                    case "FindStrings":
                        addFindStringtoolStripMenuItem.Enabled = true;
                        break;
                    case "Coordinates":
                        addCoordinatestoolStripMenuItem.Enabled = true;
                        break;
                    default:
                        break;
                }
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is Command command))
            {
                aboveToolStripMenuItem.Enabled = true;
                belowToolStripMenuItem.Enabled = true;
                Command commandCopy = command;
                if (Enum.TryParse(commandCopy.CommandId, true, out ValidCommandIds validCommandIds))
                    switch (validCommandIds)
                    {
                        case ValidCommandIds.Click:
                            if (commandCopy.Location != null)
                            {
                                tbPointX.Text = commandCopy.Location.X.ToString();
                                tbPointY.Text = commandCopy.Location.Y.ToString();
                            }
                            else
                            {
                                tbPointX.Text = "0";
                                tbPointY.Text = "0";
                            }
                            gbClick.Enabled = true;
                            gbClick.Visible = true;
                            break;
                        case ValidCommandIds.ClickWhenNotFoundInArea:
                            if (commandCopy.ImageName != null)
                            {
                                cbImageAreasImage.Text = commandCopy.ImageName;
                            }
                            else
                                cbImageAreasImage.Text = string.Empty;
                            lbImageAreaAreas.Items.Clear();
                            if (commandCopy.Areas != null)
                            {
                                foreach (SearchArea areaItem in commandCopy.Areas)
                                {
                                    lbImageAreaAreas.Items.Add(string.Format("({0}, {1}) - ({2}, {3})", areaItem.X, areaItem.Y, areaItem.X + areaItem.Width, areaItem.Y + areaItem.Height));
                                }
                            }
                            gbImageArea.Enabled = true;
                            gbImageArea.Visible = true;
                            break;
                        case ValidCommandIds.Drag:
                            if (commandCopy.Swipe != null)
                            {
                                tbDragX1.Text = commandCopy.Swipe.X1.ToString();
                                tbDragY1.Text = commandCopy.Swipe.Y1.ToString();
                                tbDragX2.Text = commandCopy.Swipe.X2.ToString();
                                tbDragY2.Text = commandCopy.Swipe.Y2.ToString();
                            }
                            else
                            {
                                tbDragX1.Text = "0";
                                tbDragY1.Text = "0";
                                tbDragX2.Text = "0";
                                tbDragY2.Text = "0";
                            }
                            if (commandCopy.Delay != null)
                            {
                                tbDragTime.Text = commandCopy.Delay.ToString();
                            }
                            else
                            {
                                tbDragTime.Text = "150";
                            }
                            gbDrag.Enabled = true;
                            gbDrag.Visible = true;
                            break;
                        case ValidCommandIds.Exit:
                            // Nothing to do here, no UI element.
                            break;
                        case ValidCommandIds.EnterLoopCoordinate:
                            if (commandCopy.Value != null)
                            {
                                if (commandCopy.Value.ToLower() == "x")
                                { rbLoopCoordX.Checked = true; }
                                else
                                { rbLoopCoordY.Checked = true; }
                            }
                            else
                            {
                                rbLoopCoordX.Checked = false;
                                rbLoopCoordY.Checked = false;
                            }
                            gbLoopCoordinate.Enabled = true;
                            gbLoopCoordinate.Visible = true;
                            break;
                        case ValidCommandIds.FindClick:
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                            else
                            {
                                cbImageNameNoWait.SelectedIndex = -1;
                                cbImageNameNoWait.Text = "";
                            }
                            if (commandCopy.IgnoreMissing != null)
                            {
                                cbIgnoreMissing.Checked = (bool)command.IgnoreMissing;
                            }
                            else
                            {
                                // Default to False
                                cbIgnoreMissing.Checked = false;
                            }
                            cbIgnoreMissing.Enabled = true;
                            cbIgnoreMissing.Visible = true;
                            gbImageName.Enabled = true;
                            gbImageName.Visible = true;
                            break;
                        case ValidCommandIds.IfExists:
                        case ValidCommandIds.IfNotExists:
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                            else
                                cbImageNameNoWait.SelectedIndex = -1;
                            gbImageName.Enabled = true;
                            gbImageName.Visible = true;
                            addCommandToolStripMenuItem.Enabled = true;
                            break;
                        case ValidCommandIds.LoopCoordinates:
                            if (commandCopy.Coordinates != null)
                                tbListName.Text = commandCopy.Coordinates;
                            else
                                tbListName.Text = string.Empty;
                            gbList.Enabled = true;
                            gbList.Visible = true;
                            addCommandToolStripMenuItem.Enabled = true;
                            break;
                        case ValidCommandIds.FindClickAndWait:
                            lbImageNames.Items.Clear();
                            if (commandCopy.ImageNames != null)
                                foreach (string item in commandCopy.ImageNames)
                                {
                                    lbImageNames.Items.Add(item);
                                }
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                lbImageNames.Items.Add(commandCopy.ImageName);
                            if (commandCopy.TimeOut != null)
                                tbImageNamesWait.Text = commandCopy.TimeOut.ToString();
                            else
                                tbImageNamesWait.Text = "";
                            gbImageNames.Enabled = true;
                            gbImageNames.Visible = true;
                            break;
                        case ValidCommandIds.LoopUntilFound:
                        case ValidCommandIds.LoopUntilNotFound:
                            lbImageNames.Items.Clear();
                            if (commandCopy.ImageNames != null)
                                foreach (string item in commandCopy.ImageNames)
                                {
                                    lbImageNames.Items.Add(item);
                                }
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                lbImageNames.Items.Add(commandCopy.ImageName);
                            if (commandCopy.TimeOut != null)
                                tbImageNamesWait.Text = commandCopy.TimeOut.ToString();
                            else
                                tbImageNamesWait.Text = "";
                            gbImageNames.Enabled = true;
                            gbImageNames.Visible = true;
                            addCommandToolStripMenuItem.Enabled = true;
                            break;
                        case ValidCommandIds.Restart:
                            // Nothing to do here, no UI element.
                            break;
                        case ValidCommandIds.RunAction:
                            if (commandCopy.ActionName != null)
                            {
                                cbPickActionAction.Text = commandCopy.ActionName;
                            }
                            else
                            {
                                cbPickActionAction.Text = string.Empty;
                            }
                            gbPickAction.Enabled = true;
                            gbPickAction.Visible = true;
                            break;
                        case ValidCommandIds.Sleep:
                            if (commandCopy.Delay != null)
                                tbDelay.Text = commandCopy.Delay.ToString();
                            else
                                tbDelay.Text = "";
                            gbSleep.Enabled = true;
                            gbSleep.Visible = true;
                            break;
                        case ValidCommandIds.StartGame:
                        case ValidCommandIds.StopGame:
                            if (commandCopy.Value != null)
                            {
                                tbAppNameAppId.Text = commandCopy.Value;
                            }
                            if (commandCopy.TimeOut != null)
                            {
                                tbAppNameTimeout.Text = commandCopy.TimeOut.ToString();
                            }
                            else
                                tbAppNameTimeout.Text = "";
                            gbAppName.Enabled = true;
                            gbAppName.Visible = true;
                            break;
                        case ValidCommandIds.WaitFor:
                            lbImageNames.Items.Clear();
                            if (commandCopy.ImageNames != null)
                                foreach (string item in commandCopy.ImageNames)
                                {
                                    lbImageNames.Items.Add(item);
                                }
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                lbImageNames.Items.Add(commandCopy.ImageName);
                            if (commandCopy.TimeOut != null)
                                tbImageNamesWait.Text = commandCopy.TimeOut.ToString();
                            else
                                tbImageNamesWait.Text = "";
                            if (commandCopy.IgnoreMissing != null)
                                cbImageNamesMissingOk.Checked = (bool)commandCopy.IgnoreMissing;
                            else
                                cbImageNamesMissingOk.Checked = false;
                            gbImageNames.Enabled = true;
                            gbImageNames.Visible = true;
                            cbImageNamesMissingOk.Enabled = true;
                            cbImageNamesMissingOk.Visible = true;
                            addCommandToolStripMenuItem.Enabled = true;
                            break;
                        case ValidCommandIds.WaitForThenClick:
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                cbImageNameWithWait.SelectedItem = commandCopy.ImageName;
                            else
                                cbImageNameWithWait.SelectedIndex = -1;
                            if (commandCopy.TimeOut != null)
                                tbTimeout.Text = commandCopy.TimeOut.ToString();
                            else
                                tbTimeout.Text = "";
                            cbImageNameMissingOk.Checked = false;
                            cbImageNameMissingOk.Enabled = false;
                            cbImageNameMissingOk.Visible = false;
                            gbImageNameAndWait.Enabled = true;
                            gbImageNameAndWait.Visible = true;
                            break;
                        case ValidCommandIds.WaitForChange:
                        case ValidCommandIds.WaitForNoChange:
                            tbWFNCWait.Text = commandCopy.TimeOut.ToString();
                            if (commandCopy.ChangeDetectArea != null)
                            {
                                tbWFNCX1.Text = commandCopy.ChangeDetectArea.X.ToString();
                                tbWFNCY1.Text = commandCopy.ChangeDetectArea.Y.ToString();
                                tbWFNCX2.Text = (commandCopy.ChangeDetectArea.X + commandCopy.ChangeDetectArea.Width).ToString();
                                tbWFNCY2.Text = (commandCopy.ChangeDetectArea.Y + commandCopy.ChangeDetectArea.Height).ToString();
                            }
                            else
                            {
                                tbWFNCX1.Text = "0";
                                tbWFNCY1.Text = "0";
                                tbWFNCX2.Text = "0";
                                tbWFNCY2.Text = "0";
                            }
                            if (commandCopy.ChangeDetectDifference != null)
                            {
                                nudWFNCDetectPercent.Value = (decimal)(commandCopy.ChangeDetectDifference * 100.0);
                            }
                            else
                            {
                                nudWFNCDetectPercent.Value = 30;
                            }
                            gbWFNC.Enabled = true;
                            gbWFNC.Visible = true;
                            break;
                        case ValidCommandIds.LoopCounter:
                            if (int.TryParse(commandCopy.Value, out _))
                                tbLoopsCounter.Text = commandCopy.Value;
                            if (commandCopy.OverrideId != null)
                                tbLoopsOverrideId.Text = commandCopy.OverrideId;
                            else
                                tbLoopsOverrideId.Text = string.Empty;
                            addCommandToolStripMenuItem.Enabled = true;
                            gbLoops.Enabled = true;
                            gbLoops.Visible = true;
                            break;
                        default:
                            MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                            break;
                    }
                else
                    MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is BotEngineClient.Action action))
            {
                gbAction.Enabled = true;
                gbAction.Visible = true;
                addCommandToolStripMenuItem.Enabled = true;
                BotEngineClient.Action actionCopy = action;
                tbActionName.Text = e.Node.Name;
                cbActionType.Text = actionCopy.ActionType;
                dtpActionTimeOfDay.Enabled = false;
                tbActionFrequency.Enabled = false;
                if (Enum.TryParse(actionCopy.ActionType, true, out ValidActionType validActionType))
                {
                    switch (validActionType)
                    {
                        case ValidActionType.Daily:
                            dtpActionTimeOfDay.Enabled = true;
                            tbActionFrequency.Enabled = false;
                            break;
                        case ValidActionType.Scheduled:
                            dtpActionTimeOfDay.Enabled = false;
                            tbActionFrequency.Enabled = true;
                            break;
                        default:
                            break;
                    }
                }
                if (actionCopy.DailyScheduledTime != null)
                {
                    if (actionCopy.DailyScheduledTime != DateTime.MinValue)
                    {
                        dtpActionTimeOfDay.Text = actionCopy.DailyScheduledTime.ToString();
                        dtpActionTimeOfDay.Checked = true;
                    }
                    else
                    {
                        dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtpActionTimeOfDay.Checked = false;
                    }
                }
                else
                {
                    dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                    dtpActionTimeOfDay.Checked = false;
                }
                tbActionFrequency.Text = actionCopy.Frequency.ToString();
                if (e.Node.Parent.Name == "SystemActions")
                    cbActionType.Enabled = false;
                else
                    cbActionType.Enabled = true;
                cbActionBefore.Text = actionCopy.BeforeAction;
                cbActionAfter.Text = actionCopy.AfterAction;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is FindString findString))
            {
                gbFindText.Enabled = true;
                gbFindText.Visible = true;
                addFindStringtoolStripMenuItem.Enabled = true;
                tbFindTextBackTolerance.Text = findString.BackgroundTolerance.ToString();
                tbFindTextTextTolerance.Text = findString.TextTolerance.ToString();
                tbFindTextName.Text = e.Node.Name;
                tbFindTextSearch.Text = findString.SearchString;
                tbFindTextSearchX1.Text = findString.SearchArea.X.ToString();
                tbFindTextSearchY1.Text = findString.SearchArea.Y.ToString();
                tbFindTextSearchX2.Text = (findString.SearchArea.X + findString.SearchArea.Width).ToString();
                tbFindTextSearchY2.Text = (findString.SearchArea.Y + findString.SearchArea.Height).ToString();
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is XYCoords coords))
            {
                if (coords != null)
                {
                    tbPointX.Text = coords.X.ToString();
                    tbPointY.Text = coords.Y.ToString();
                }
                else
                {
                    tbPointX.Text = "0";
                    tbPointY.Text = "0";
                }
                aboveToolStripMenuItem.Enabled = true;
                belowToolStripMenuItem.Enabled = true;
                gbClick.Enabled = true;
                gbClick.Visible = true;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is BotEngineClient.ActionActivity actionActivity))
            {
                tbActionOverrideEnabled.Checked = actionActivity.ActionEnabled;
                tbActionOverrideName.Text = e.Node.Name;
                if (actionActivity.LastRun != DateTime.MinValue)
                {
                    dtpActionOverrideLastRun.Value = actionActivity.LastRun;
                }
                else
                {
                    dtpActionOverrideLastRun.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                }
                if (actionActivity.DailyScheduledTime != null)
                {
                    if (actionActivity.DailyScheduledTime != DateTime.MinValue)
                    {
                        dtptbActionOverrideTimeOfDay.Value = (DateTime)actionActivity.DailyScheduledTime;
                        dtptbActionOverrideTimeOfDay.Checked = true;
                    }
                    else
                    {
                        dtptbActionOverrideTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtptbActionOverrideTimeOfDay.Checked = false;
                    }
                }
                else
                {
                    dtptbActionOverrideTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                    dtptbActionOverrideTimeOfDay.Checked = false;
                }
                if (actionActivity.Frequency != null)
                {
                    tbActionFrequency.Text = actionActivity.Frequency.ToString();
                }
                else
                {
                    tbActionFrequency.Text = string.Empty;
                }
                lvActionOverridesOverride.Items.Clear();
                if (actionActivity.CommandValueOverride != null)
                {
                    foreach (KeyValuePair<string, string> item in actionActivity.CommandValueOverride)
                    {
                        ListViewItem lvItem = new ListViewItem();
                        lvItem.Text = item.Key;
                        ListViewItem.ListViewSubItem lvSubItem = new ListViewItem.ListViewSubItem();
                        lvSubItem.Text = item.Value;
                        lvItem.SubItems.Add(lvSubItem);
                        lvActionOverridesOverride.Items.Add(lvItem);
                    }
                }
                tbActionOverrideOverrideId.Text = string.Empty;
                tbActionOverrideValue.Text = string.Empty;
                gbActionOverride.Enabled = true;
                gbActionOverride.Visible = true;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is List<XYCoords>))
            {
                tbListName.Text = ActiveTreeNode.Name;
                addCoordinatestoolStripMenuItem.Enabled = true;
                gbList.Enabled = true;
                gbList.Visible = true;
            }

            ChangePending = false;
            btnUpdate.Enabled = false;
        }

        /// <summary>
        /// Ensures that there aren't pending changes before allowing navigation within the tvBotData tree view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvBotData_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (ChangePending)
            {
                DialogResult answer = MessageBox.Show("There are pending changes.  Keep Them?", "Pending Changes", MessageBoxButtons.YesNoCancel);
                switch (answer)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        BtnUpdate_Click(sender, null);
                        e.Cancel = false;
                        break;
                    case DialogResult.No:
                        e.Cancel = false;
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Reads the content in the tvBotData back into the global gameConfig
        /// </summary>
        private void RefreshGameConfig()
        {
            gameConfig = new BOTConfig {
                FileId = loadedFileType.ToString(),
                FindStrings = new Dictionary<string, FindString>(),
                SystemActions = new Dictionary<string, BotEngineClient.Action>(),
                Actions = new Dictionary<string, BotEngineClient.Action>()
            };
            foreach (TreeNode parent in tvBotData.Nodes)
            {
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "FindStrings")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        gameConfig.FindStrings.Add(child.Name, (FindString)child.Tag);
                    }
                }
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "SystemActions")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        BotEngineClient.Action sourceAction = (BotEngineClient.Action)child.Tag;
                        BotEngineClient.Action newAction = new BotEngineClient.Action {
                            ActionType = sourceAction.ActionType,
                            AfterAction = sourceAction.AfterAction,
                            BeforeAction = sourceAction.BeforeAction
                        };
                        if (child.Nodes.Count != 0)
                        {
                            int commandNumber = 10;
                            List<Command> childCommands = new List<Command>();
                            LoadCommandList(childCommands, child, ref commandNumber);
                            newAction.Commands = childCommands;
                        }
                        newAction.DailyScheduledTime = sourceAction.DailyScheduledTime;
                        newAction.Frequency = sourceAction.Frequency;
                        gameConfig.SystemActions.Add(child.Name, newAction);
                    }
                }
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "Actions")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        BotEngineClient.Action sourceAction = (BotEngineClient.Action)child.Tag;
                        BotEngineClient.Action newAction = new BotEngineClient.Action {
                            ActionType = sourceAction.ActionType,
                            AfterAction = sourceAction.AfterAction,
                            BeforeAction = sourceAction.BeforeAction
                        };
                        if (child.Nodes.Count != 0)
                        {
                            int commandNumber = 10;
                            List<Command> childCommands = new List<Command>();
                            LoadCommandList(childCommands, child, ref commandNumber);
                            newAction.Commands = childCommands;
                        }
                        newAction.DailyScheduledTime = sourceAction.DailyScheduledTime;
                        newAction.Frequency = sourceAction.Frequency;
                        gameConfig.Actions.Add(child.Name, newAction);
                    }
                }
            }
        }

        /// <summary>
        /// Connects ADB server (if not connected), and refreshes the device list.
        /// </summary>
        private bool RefreshDeviceList()
        {
            if (server == null)
            {
                server = new AdbServer();
                StartServerResult result = server.StartServer(AppDomain.CurrentDomain.BaseDirectory + @"\ADB\adb.exe", restartServerIfNewer: true);
                if (result != StartServerResult.AlreadyRunning)
                {
                    Thread.Sleep(1500);
                    AdbServerStatus status = server.GetStatus();
                    if (!status.IsRunning)
                    {
                        MessageBox.Show("Unable to start ADB server");
                        return false;
                    }
                }
            }

            AdbClient client = new AdbClient();
            List<DeviceData> devices = client.GetDevices();

            if (devicesList == null)
            {
                devicesList = new List<string>();
            }
            else
            {
                devicesList.Clear();
            }
            foreach (DeviceData device in devices)
            {
                string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                string deviceId = string.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
                devicesList.Add(deviceId);
            }
            return true;
        }

        /// <summary>
        /// Setup a file watcher on the file being edited, to warn user that it has been changed.
        /// Most useful on DeviceConfig files, as they are likely to change every 10 minutes.
        /// </summary>
        /// <param name="fileName"></param>
        private void EnableFileWatcher(string fileName)
        {
            if (fileWatcher != null)
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }

            fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(fileName)) {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.Attributes,
                Filter = "*.json"
            };
            fileWatcher.Changed += ReloadJSONConfig;
            fileWatcher.Renamed += ReloadJSONConfig;
            fileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Updates variables so that the UI knows to reload the data, on next click.
        /// Would be nicer to force it, but the FileWatcher event needs static, and I haven't worked out how to force a reload.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ReloadJSONConfig(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.FullPath).ToLower() == Path.GetFileName(JsonFileName).ToLower())
            {
                fileWatcher.EnableRaisingEvents = false;
                if (MessageBox.Show(string.Format("{0} has changed, reload?", JsonFileName), "File Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ReloadTreeViewRequired = true;
                }
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Ensure that a GroupBox is in the corect position, and hidden
        /// </summary>
        /// <param name="item"></param>
        private void ResetGroupBox(GroupBox item)
        {
            item.Top = 5;
            item.Left = 5;
            item.Dock = DockStyle.Top;
            item.Enabled = false;
            item.Visible = false;
        }

        /// <summary>
        /// Set the text to show on the tvBotData for commands.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string GetCommandIdDisplayText(Command command)
        {
            string childText = command.CommandId;
            if (Enum.TryParse(command.CommandId, true, out ValidCommandIds validCommandIds))
                switch (validCommandIds)
                {
                    case ValidCommandIds.Click:
                        if (command.Location != null)
                            childText = string.Format("{0} ({1},{2})", command.CommandId, command.Location.X, command.Location.Y);
                        break;
                    case ValidCommandIds.Drag:
                        if (command.Swipe != null)
                            childText = string.Format("{0} ({1}, {2}) - ({3}, {4})", command.CommandId, command.Swipe.X1, command.Swipe.Y1, command.Swipe.X2, command.Swipe.Y2);
                        break;
                    case ValidCommandIds.EnterLoopCoordinate:
                        if (command.Value != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.Value);
                        break;
                    case ValidCommandIds.LoopCoordinates:
                        if (command.Coordinates != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.Coordinates);
                        break;
                    case ValidCommandIds.RunAction:
                        if (command.ActionName != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.ActionName);
                        break;
                    case ValidCommandIds.Sleep:
                        childText = string.Format("{0} ({1})", command.CommandId, command.Delay);
                        break;
                    case ValidCommandIds.IfExists:
                    case ValidCommandIds.IfNotExists:
                    case ValidCommandIds.FindClick:
                    case ValidCommandIds.FindClickAndWait:
                    case ValidCommandIds.LoopUntilFound:
                    case ValidCommandIds.LoopUntilNotFound:
                    case ValidCommandIds.WaitFor:
                    case ValidCommandIds.WaitForThenClick:
                        if (command.ImageName != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.ImageName);
                        else
                            childText = string.Format("{0} (list)", command.CommandId);
                        break;
                    case ValidCommandIds.WaitForChange:
                    case ValidCommandIds.WaitForNoChange:
                        if (command.ChangeDetectArea != null)
                            childText = string.Format("{0} ({1}, {2}) - ({3}, {4})", command.CommandId, command.ChangeDetectArea.X, command.ChangeDetectArea.Y, command.ChangeDetectArea.X + command.ChangeDetectArea.Width, command.ChangeDetectArea.Y + command.ChangeDetectArea.Height);
                        break;
                    case ValidCommandIds.LoopCounter:
                        if (!string.IsNullOrEmpty(command.Value))
                            childText = string.Format("{0} ({1})", command.CommandId, command.Value);
                        else
                            childText = string.Format("{0} (undefined)", command.CommandId);
                        break;
                    case ValidCommandIds.ClickWhenNotFoundInArea:
                    case ValidCommandIds.Exit:
                    case ValidCommandIds.Restart:
                    case ValidCommandIds.StartGame:
                    case ValidCommandIds.StopGame:
                    default:
                        childText = command.CommandId;
                        break;
                }
            return childText;
        }

        /// <summary>
        /// Hides all the GUI elements, and disables menu items.
        /// </summary>
        private void ResetEditFormItems()
        {
            addFindStringtoolStripMenuItem.Enabled = false;
            addActionToolStripMenuItem.Enabled = false;
            addCoordinatestoolStripMenuItem.Enabled = false;
            addCommandToolStripMenuItem.Enabled = false;
            aboveToolStripMenuItem.Enabled = false;
            belowToolStripMenuItem.Enabled = false;
            upToolStripMenuItem.Enabled = false;
            downToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            gbClick.Enabled = false;
            gbClick.Visible = false;
            gbDrag.Enabled = false;
            gbDrag.Visible = false;
            gbImageName.Enabled = false;
            gbImageName.Visible = false;
            cbIgnoreMissing.Enabled = false;
            cbIgnoreMissing.Visible = false;
            gbImageNameAndWait.Enabled = false;
            gbImageNameAndWait.Visible = false;
            gbImageNames.Enabled = false;
            gbImageNames.Visible = false;
            gbLoopCoordinate.Visible = false;
            gbLoopCoordinate.Enabled = false;
            gbSleep.Enabled = false;
            gbSleep.Visible = false;
            gbAction.Visible = false;
            gbAction.Enabled = false;
            gbFindText.Visible = false;
            gbFindText.Enabled = false;
            gbWFNC.Visible = false;
            gbWFNC.Enabled = false;
            gbAppControl.Visible = false;
            gbAppControl.Enabled = false;
            gbAppName.Visible = false;
            gbAppName.Enabled = false;
            gbActionOverride.Visible = false;
            gbActionOverride.Enabled = false;
            gbPickAction.Enabled = false;
            gbPickAction.Visible = false;
            gbImageArea.Enabled = false;
            gbImageArea.Visible = false;
            gbList.Enabled = false;
            gbList.Visible = false;
            btnUpdate.Enabled = false;
            gbLoops.Enabled = false;
            gbLoops.Visible = false;
        }

        /// <summary>
        /// Sets the variables that control pending changes, and ensures appropriate menu items are available.
        /// </summary>
        private void SetChangePending()
        {
            ChangePending = true;
            btnUpdate.Enabled = true;
        }

        #endregion

        #region Form Event listeners
        /// <summary>
        /// Fires when closing the form to ensure that a prompt is given if changes are pending.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScriptEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ChangePending || UnsavedChanges)
            {
                if (MessageBox.Show("There are Unsaved or Pending Changes, Exit?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Other Event Listeners
        /// <summary>
        /// Takes data on the form, and updates the tvBotData's names, and Tag with data from the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var selectedTag = ActiveTreeNode.Tag;
            if (selectedTag != null)
            {
                if (selectedTag is FindString findTag)
                {
                    ActiveTreeNode.Name = tbFindTextName.Text;
                    if (tbFindTextBackTolerance.Text.Length == 0 || tbFindTextTextTolerance.Text.Length == 0
                        || tbFindTextSearchY2.Text.Length == 0 || tbFindTextSearch.Text.Length == 0
                        || tbFindTextSearchX1.Text.Length == 0 || tbFindTextSearchY1.Text.Length == 0
                        || tbFindTextSearchX2.Text.Length == 0)
                    {
                        MessageBox.Show("Required fields aren't populated.");
                        return;
                    }
                    else
                    {
                        if (cbImageNameNoWait.Items.Contains(ActiveTreeNode.Name))
                        {
                            cbImageNameNoWait.Items.Remove(ActiveTreeNode.Name);
                            cbImageNameWithWait.Items.Remove(ActiveTreeNode.Name);
                            cbImageNamesForList.Items.Remove(ActiveTreeNode.Name);
                            cbImageAreasImage.Items.Remove(ActiveTreeNode.Name);
                        }
                        cbImageNameNoWait.Items.Add(tbFindTextName.Text);
                        cbImageNameWithWait.Items.Add(tbFindTextName.Text);
                        cbImageNamesForList.Items.Add(tbFindTextName.Text);
                        cbImageAreasImage.Items.Add(tbFindTextName.Text);

                        ActiveTreeNode.Name = tbFindTextName.Text;
                        ActiveTreeNode.Text = tbFindTextName.Text;
                        findTag.BackgroundTolerance = float.Parse(tbFindTextBackTolerance.Text);
                        findTag.TextTolerance = float.Parse(tbFindTextTextTolerance.Text);
                        findTag.SearchString = tbFindTextSearch.Text;
                        findTag.SearchArea.X = int.Parse(tbFindTextSearchX1.Text);
                        findTag.SearchArea.Y = int.Parse(tbFindTextSearchY1.Text);
                        findTag.SearchArea.Width = int.Parse(tbFindTextSearchX2.Text) - findTag.SearchArea.X;
                        findTag.SearchArea.Height = int.Parse(tbFindTextSearchY2.Text) - findTag.SearchArea.Y;
                    }
                }
                else if (selectedTag is BotEngineClient.Action botAction)
                {
                    if (cbActionAfter.Items.Contains(ActiveTreeNode.Name))
                    {
                        cbActionAfter.Items.Remove(ActiveTreeNode.Name);
                        cbActionBefore.Items.Remove(ActiveTreeNode.Name);
                        cbPickActionAction.Items.Remove(ActiveTreeNode.Name);
                    }
                    cbActionAfter.Items.Add(tbActionName.Text);
                    cbActionBefore.Items.Add(tbActionName.Text);
                    cbPickActionAction.Items.Add(tbActionName.Text);

                    ActiveTreeNode.Name = tbActionName.Text;
                    ActiveTreeNode.Text = tbActionName.Text;

                    botAction.ActionType = cbActionType.Text;
                    botAction.AfterAction = cbActionAfter.Text;
                    botAction.BeforeAction = cbActionBefore.Text;
                    botAction.Frequency = null;
                    botAction.DailyScheduledTime = null;
                    if (Enum.TryParse(botAction.ActionType, true, out ValidActionType validActionType))
                    {
                        switch (validActionType)
                        {
                            case ValidActionType.Daily:
                                botAction.Frequency = null;
                                botAction.DailyScheduledTime = dtpActionTimeOfDay.Value;
                                break;
                            case ValidActionType.Scheduled:
                                if (tbActionFrequency.Text.Length > 0)
                                    botAction.Frequency = int.Parse(tbActionFrequency.Text);
                                else
                                {
                                    MessageBox.Show("Frequency is required when Scheduled is selected.");
                                    botAction.Frequency = null;
                                    return;
                                }
                                botAction.DailyScheduledTime = null;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (selectedTag is Command commandCopy)
                {
                    if (Enum.TryParse(commandCopy.CommandId, true, out ValidCommandIds validCommandIds))
                    {
                        switch (validCommandIds)
                        {
                            case ValidCommandIds.Click:
                                if (commandCopy.Location == null)
                                {
                                    commandCopy.Location = new XYCoords();
                                }
                                if (tbPointX.Text.Length == 0 || tbPointY.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }
                                commandCopy.Location.X = int.Parse(tbPointX.Text);
                                commandCopy.Location.Y = int.Parse(tbPointY.Text);
                                break;
                            case ValidCommandIds.ClickWhenNotFoundInArea:
                                commandCopy.ImageName = cbImageAreasImage.Text;
                                commandCopy.Areas.Clear();
                                char[] delimiters = { ',', '-' };
                                foreach (string item in lbImageAreaAreas.Items)
                                {
                                    string[] values = item.Replace("(", "").Replace(")", "").Replace(" ", "").Split(delimiters);
                                    SearchArea searchArea = new SearchArea {
                                        X = int.Parse(values[0]),
                                        Y = int.Parse(values[1])
                                    };
                                    searchArea.Width = int.Parse(values[2]) - searchArea.X;
                                    searchArea.Height = int.Parse(values[3]) - searchArea.Y;
                                    commandCopy.Areas.Add(searchArea);
                                }
                                break;
                            case ValidCommandIds.Drag:
                                if (tbDragX1.Text.Length == 0 || tbDragX2.Text.Length == 0
                                    || tbDragY1.Text.Length == 0 || tbDragY2.Text.Length == 0
                                    || tbDragTime.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }
                                if (commandCopy.Swipe == null)
                                {
                                    commandCopy.Swipe = new SwipeCoords();
                                }
                                commandCopy.Swipe.X1 = int.Parse(tbDragX1.Text);
                                commandCopy.Swipe.Y1 = int.Parse(tbDragY1.Text);
                                commandCopy.Swipe.X2 = int.Parse(tbDragX2.Text);
                                commandCopy.Swipe.Y2 = int.Parse(tbDragY2.Text);
                                commandCopy.Delay = int.Parse(tbDragTime.Text);
                                break;
                            case ValidCommandIds.Exit:
                                break;
                            case ValidCommandIds.EnterLoopCoordinate:
                                if (rbLoopCoordX.Checked)
                                {
                                    commandCopy.Value = "X";
                                }
                                else
                                {
                                    commandCopy.Value = "Y";
                                }
                                break;
                            case ValidCommandIds.FindClick:
                                commandCopy.ImageName = (string)cbImageNameNoWait.SelectedItem;
                                commandCopy.IgnoreMissing = cbIgnoreMissing.Checked;
                                break;
                            case ValidCommandIds.IfExists:
                            case ValidCommandIds.IfNotExists:
                                commandCopy.ImageName = (string)cbImageNameNoWait.SelectedItem;
                                break;
                            case ValidCommandIds.LoopCoordinates:
                                commandCopy.Coordinates = tbListName.Text;
                                break;
                            case ValidCommandIds.LoopUntilFound:
                            case ValidCommandIds.LoopUntilNotFound:
                                if (tbImageNamesWait.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }
                                if (lbImageNames.Items.Count == 0)
                                {
                                    commandCopy.ImageName = null;
                                    commandCopy.ImageNames = null;
                                }
                                else if (lbImageNames.Items.Count == 1)
                                {
                                    commandCopy.ImageName = (string)lbImageNames.Items[0];
                                    commandCopy.ImageNames = null;
                                }
                                else
                                {
                                    commandCopy.ImageName = null;
                                    if (commandCopy.ImageNames == null)
                                    {
                                        commandCopy.ImageNames = new List<string>();
                                    }
                                    foreach (string item in lbImageNames.Items)
                                    {
                                        commandCopy.ImageNames.Add(item);
                                    }
                                }
                                commandCopy.TimeOut = int.Parse(tbImageNamesWait.Text);
                                break;
                            case ValidCommandIds.Restart:
                                break;
                            case ValidCommandIds.RunAction:
                                commandCopy.ActionName = cbPickActionAction.Text;
                                break;
                            case ValidCommandIds.Sleep:
                                if (tbDelay.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }

                                commandCopy.Delay = int.Parse(tbDelay.Text);
                                break;
                            case ValidCommandIds.StartGame:
                            case ValidCommandIds.StopGame:
                                if (tbAppNameTimeout.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }

                                commandCopy.Value = tbAppNameAppId.Text;
                                commandCopy.TimeOut = int.Parse(tbAppNameTimeout.Text);
                                break;
                            case ValidCommandIds.FindClickAndWait:
                                if (tbImageNamesWait.Text.Length == 0)
                                {
                                    MessageBox.Show("Wait Time required field isn't populated.");
                                    return;
                                }

                                if (lbImageNames.Items.Count < 1)
                                {
                                    MessageBox.Show("Image Names required field isn't populated.");
                                    return;
                                }
                                if (lbImageNames.Items.Count == 1)
                                {
                                    commandCopy.ImageName = (string)lbImageNames.Items[0];
                                    commandCopy.ImageNames = null;
                                }
                                else
                                {
                                    commandCopy.ImageName = null;
                                    commandCopy.ImageNames = new List<string>();
                                    foreach (string item in lbImageNames.Items)
                                    {
                                        commandCopy.ImageNames.Add(item);
                                    }
                                }
                                commandCopy.TimeOut = int.Parse(tbImageNamesWait.Text);
                                break;
                            case ValidCommandIds.WaitFor:
                                if (tbImageNamesWait.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }
                                if (lbImageNames.Items.Count == 0)
                                {
                                    commandCopy.ImageName = null;
                                    commandCopy.ImageNames = null;
                                }
                                else if (lbImageNames.Items.Count == 1)
                                {
                                    commandCopy.ImageName = (string)lbImageNames.Items[0];
                                    commandCopy.ImageNames = null;
                                }
                                else
                                {
                                    commandCopy.ImageName = null;
                                    if (commandCopy.ImageNames == null)
                                    {
                                        commandCopy.ImageNames = new List<string>();
                                    }
                                    foreach (string item in lbImageNames.Items)
                                    {
                                        commandCopy.ImageNames.Add(item);
                                    }
                                }
                                commandCopy.TimeOut = int.Parse(tbImageNamesWait.Text);
                                commandCopy.IgnoreMissing = cbImageNamesMissingOk.Checked;
                                break;
                            case ValidCommandIds.WaitForThenClick:
                                if (tbTimeout.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }

                                commandCopy.ImageName = (string)cbImageNameWithWait.SelectedItem;
                                commandCopy.TimeOut = int.Parse(tbTimeout.Text);
                                break;
                            case ValidCommandIds.WaitForChange:
                            case ValidCommandIds.WaitForNoChange:
                                if (tbWFNCWait.Text.Length == 0
                                    || tbWFNCX1.Text.Length == 0 || tbWFNCX2.Text.Length == 0
                                    || tbWFNCY1.Text.Length == 0 || tbWFNCY2.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }

                                commandCopy.TimeOut = int.Parse(tbWFNCWait.Text);
                                if (commandCopy.ChangeDetectArea == null)
                                {
                                    commandCopy.ChangeDetectArea = new SearchArea();
                                }
                                commandCopy.ChangeDetectArea.X = int.Parse(tbWFNCX1.Text);
                                commandCopy.ChangeDetectArea.Y = int.Parse(tbWFNCY1.Text);
                                commandCopy.ChangeDetectArea.Width = int.Parse(tbWFNCX2.Text) - commandCopy.ChangeDetectArea.X;
                                commandCopy.ChangeDetectArea.Height = int.Parse(tbWFNCY2.Text) - commandCopy.ChangeDetectArea.Y;
                                commandCopy.ChangeDetectDifference = (float)nudWFNCDetectPercent.Value / 100.0f;
                                break;
                            case ValidCommandIds.LoopCounter:
                                if (tbLoopsCounter.Text.Length == 0)
                                {
                                    MessageBox.Show("Required fields aren't populated.");
                                    return;
                                }
                                commandCopy.Value = tbLoopsCounter.Text;
                                if (tbLoopsOverrideId.Text.Length > 0)
                                {
                                    commandCopy.OverrideId = tbLoopsOverrideId.Text;
                                }
                                else
                                {
                                    commandCopy.OverrideId = null;
                                }
                                break;
                            default:
                                MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                                break;
                        }
                        ActiveTreeNode.Text = GetCommandIdDisplayText(commandCopy);
                    }
                    else
                        MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                }
                else if (selectedTag is ActionActivity actionActivity)
                {
                    actionActivity.ActionEnabled = tbActionOverrideEnabled.Checked;
                    actionActivity.LastRun = dtpActionOverrideLastRun.Value;
                    if (tbActionOverrideFrequency.Text == string.Empty)
                        actionActivity.Frequency = null;
                    else
                    {
                        if (tbActionOverrideFrequency.Text.Length == 0)
                            actionActivity.Frequency = 4800;
                        else
                            actionActivity.Frequency = int.Parse(tbActionOverrideFrequency.Text);
                    }
                    if (dtptbActionOverrideTimeOfDay.Checked)
                        actionActivity.DailyScheduledTime = dtptbActionOverrideTimeOfDay.Value;
                    else
                        actionActivity.DailyScheduledTime = null;
                    if (lvActionOverridesOverride.Items.Count > 0)
                    {
                        if (actionActivity.CommandValueOverride == null)
                        {
                            actionActivity.CommandValueOverride = new Dictionary<string, string>();
                        }
                        actionActivity.CommandValueOverride.Clear();
                        foreach (ListViewItem item in lvActionOverridesOverride.Items)
                        {
                            actionActivity.CommandValueOverride.Add(item.Text, item.SubItems[1].Text);
                        }
                    }
                    else
                    {
                        actionActivity.CommandValueOverride = null;
                    }
                    ActiveTreeNode.Text = string.Format("{0} - {1}", tbActionOverrideName.Text, actionActivity.ActionEnabled ? "Enabled" : "Disabled");
                }
                else if (selectedTag is XYCoords coords)
                {
                    if (tbPointX.Text.Length == 0 || tbPointY.Text.Length == 0)
                    {
                        MessageBox.Show("Required fields aren't populated.");
                        return;
                    }
                    if (coords == null)
                    {
                        coords = new XYCoords();
                    }
                    coords.X = int.Parse(tbPointX.Text);
                    coords.Y = int.Parse(tbPointY.Text);
                    ActiveTreeNode.Text = string.Format("({0}, {1})", coords.X, coords.Y);
                }
                else if (selectedTag is List<XYCoords>)
                {
                    ActiveTreeNode.Text = tbListName.Text;
                    ActiveTreeNode.Name = tbListName.Text;
                }
            }
            UnsavedChanges = true;
            ChangePending = false;
            btnUpdate.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Adds a new item at the end of list boc lbImageNames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddImageNames_Click(object sender, EventArgs e)
        {
            SetChangePending();
            lbImageNames.Items.Add(cbImageNamesForList.SelectedItem.ToString());
        }

        /// <summary>
        /// Removes the selected item from list box lbImageNames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoveImageNames_Click(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItems.Count > 0)
            {
                SetChangePending();
                lbImageNames.Items.RemoveAt(lbImageNames.SelectedIndex);
            }
        }

        /// <summary>
        /// Adds an item to the list box lbImageAreaAreas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtImageAreaAdd_Click(object sender, EventArgs e)
        {
            SetChangePending();
            lbImageAreaAreas.Items.Add(string.Format("({0}, {1}) - ({2}, {3})", tbImageAreasX.Text, tbImageAreasY.Text, tbImageAreasW.Text, tbImageAreasH.Text));
        }

        /// <summary>
        /// Removes an item from the list box lbImageAreaAreas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtImageAreaRemove_Click(object sender, EventArgs e)
        {
            if (lbImageAreaAreas.SelectedItems.Count > 0)
            {
                SetChangePending();
                lbImageAreaAreas.Items.RemoveAt(lbImageAreaAreas.SelectedIndex);
            }
        }

        /// <summary>
        /// Pastes the values from the ClipBoard into a FindString, provided they are of the correct structure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPasteFindText_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboard = Clipboard.GetText();
                try
                {
                    JsonDocumentOptions documentOptions = new JsonDocumentOptions {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip
                    };
                    JsonNodeOptions nodeOptions = new JsonNodeOptions {
                        PropertyNameCaseInsensitive = false
                    };
                    JsonNode jsonClipboard = JsonNode.Parse(clipboard, nodeOptions, documentOptions);
                    if (jsonClipboard is JsonObject jsonObject)
                    {
                        if (jsonObject.ContainsKey("SearchString"))
                        {
                            tbFindTextSearch.Text = jsonObject["SearchString"].GetValue<JsonElement>().GetString();
                        }
                        if (jsonObject.ContainsKey("searchArea"))
                        {
                            tbFindTextSearchX1.Text = jsonObject["searchArea"].AsObject()["X"].GetValue<JsonElement>().GetInt32().ToString();
                            tbFindTextSearchY1.Text = jsonObject["searchArea"].AsObject()["Y"].GetValue<JsonElement>().GetInt32().ToString();
                            tbFindTextSearchX2.Text = (jsonObject["searchArea"].AsObject()["width"].GetValue<JsonElement>().GetInt32() + int.Parse(tbFindTextSearchX1.Text)).ToString();
                            tbFindTextSearchY2.Text = (jsonObject["searchArea"].AsObject()["height"].GetValue<JsonElement>().GetInt32() + int.Parse(tbFindTextSearchY1.Text)).ToString();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Clipboard content not recognised.");
                }
            }
            else
                MessageBox.Show("Clipboard content not recognised.");
        }

        /// <summary>
        /// Loads the edit box to allow a new FindString to be generated.  On OK from the Dialog, values are populated back to this form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFindTextGenerate_Click(object sender, EventArgs e)
        {
            FindTextEdit fte = new FindTextEdit();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                tbFindTextSearch.Text = fte.SearchText;
                tbFindTextSearchX1.Text = fte.SearchRectangle.X.ToString();
                tbFindTextSearchY1.Text = fte.SearchRectangle.Y.ToString();
                tbFindTextSearchX2.Text = (fte.SearchRectangle.X + fte.SearchRectangle.Width).ToString();
                tbFindTextSearchY2.Text = (fte.SearchRectangle.Y + fte.SearchRectangle.Height).ToString();
            }
        }

        /// <summary>
        /// Most form items when changed will have this as the event listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllFields_TextChanged(object sender, EventArgs e)
        {
            SetChangePending();
        }

        /// <summary>
        /// When the Action Type combo box changes, update all the enables on the rest of that group box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbActionType_TextChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse(cbActionType.Text, true, out ValidActionType validActionType))
            {
                switch (validActionType)
                {
                    case ValidActionType.Adhoc:
                    case ValidActionType.Always:
                        tbActionFrequency.Enabled = false;
                        dtpActionTimeOfDay.Enabled = false;
                        tbActionFrequency.Text = string.Empty;
                        dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtpActionTimeOfDay.Checked = false;
                        break;
                    case ValidActionType.Daily:
                        tbActionFrequency.Enabled = false;
                        dtpActionTimeOfDay.Enabled = true;
                        tbActionFrequency.Text = string.Empty;
                        dtpActionTimeOfDay.Checked = true;
                        break;
                    case ValidActionType.Scheduled:
                        tbActionFrequency.Enabled = true;
                        dtpActionTimeOfDay.Enabled = false;
                        dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtpActionTimeOfDay.Checked = false;
                        break;
                    case ValidActionType.System:
                        tbActionFrequency.Enabled = false;
                        dtpActionTimeOfDay.Enabled = false;
                        tbActionFrequency.Text = string.Empty;
                        dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtpActionTimeOfDay.Checked = false;
                        break;
                    default:
                        tbActionFrequency.Enabled = false;
                        dtpActionTimeOfDay.Enabled = false;
                        tbActionFrequency.Text = string.Empty;
                        dtpActionTimeOfDay.Value = new DateTime(2001, 01, 01, 10, 00, 00);
                        dtpActionTimeOfDay.Checked = false;
                        break;
                }
            }
            SetChangePending();
        }

        /// <summary>
        /// Ensure that the combo box has the correct values, based on the list box lbImageNames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbImageNames_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItem != null)
                cbImageNamesForList.Text = lbImageNames.SelectedItem.ToString();
        }

        /// <summary>
        /// When the selected item changes in lbImageAreaAreas, update the edit boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbImageAreaAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbImageAreaAreas.SelectedItem != null)
            {
                char[] delimiters = { ',', '-' };
                string[] values = lbImageAreaAreas.SelectedItem.ToString().Replace("(","").Replace(")","").Replace(" ","").Split(delimiters);
                tbImageAreasX.Text = values[0];
                tbImageAreasY.Text = values[1];
                tbImageAreasW.Text = values[2];
                tbImageAreasH.Text = values[3];
            }
        }

        /// <summary>
        /// When the selected item changes in lvActionOverridesOverride, update the edit boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvActionOverridesOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvActionOverridesOverride.SelectedItems.Count > 0)
            {
                tbActionOverrideOverrideId.Text = lvActionOverridesOverride.SelectedItems[0].Text;
                tbActionOverrideValue.Text = lvActionOverridesOverride.SelectedItems[0].SubItems[1].Text;
            }
        }

        /// <summary>
        /// Add the values from the text boxes into lvActionOverridesOverride
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btActionOverridesAdd_Click(object sender, EventArgs e)
        {
            ListViewItem lvItem = new ListViewItem();
            lvItem.Text = tbActionOverrideOverrideId.Text;
            ListViewItem.ListViewSubItem lvSubItem = new ListViewItem.ListViewSubItem();
            lvSubItem.Text = tbActionOverrideValue.Text;
            lvItem.SubItems.Add(lvSubItem);
            lvActionOverridesOverride.Items.Add(lvItem);
            SetChangePending();
        }

        /// <summary>
        /// Apply the changes into the selected item in lvActionOverridesOverride
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btActionOverridesEdit_Click(object sender, EventArgs e)
        {
            if (lvActionOverridesOverride.SelectedItems.Count > 0)
            {
                lvActionOverridesOverride.SelectedItems[0].Text = tbActionOverrideOverrideId.Text;
                lvActionOverridesOverride.SelectedItems[0].SubItems[1].Text = tbActionOverrideValue.Text;
            }
            SetChangePending();
        }

        /// <summary>
        /// Remove the active record from lvActionOverridesOverride
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btActionOverridesRemove_Click(object sender, EventArgs e)
        {
            if (lvActionOverridesOverride.SelectedItems.Count > 0)
            {
                lvActionOverridesOverride.SelectedItems[0].Remove();
            }
            SetChangePending();
        }

        #endregion

    }
}
