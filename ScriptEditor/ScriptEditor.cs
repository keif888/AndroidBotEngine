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
#endregion

namespace ScriptEditor
{
    public partial class ScriptEditor : Form
    {
        #region Privates
        private BOTConfig gameConfig;
        private BOTDeviceConfig deviceConfig;
        private BOTListConfig listConfig;
        private bool ChangePending;
        private bool UnsavedChanges;
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
        private AdbServer? server;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier
        private JsonHelper.ConfigFileType loadedFileType;
        private string JsonFileName;
        private TreeNode ActiveTreeNode;
        #endregion

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

            this.Size = new Size(800, 485);
            splitContainer1.SplitterDistance = 320;
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.Error;
            ActiveTreeNode = null;
            btnUpdate.Enabled = false;
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

        #region Load Files
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
                JsonHelper jsonHelper = new JsonHelper();
                switch (jsonHelper.GetFileType(fileName))
                {
                    case JsonHelper.ConfigFileType.DeviceConfig:
                        if (jsonHelper.ValidateDeviceConfigStructure(fileName))
                        {
                            LoadDeviceConfigFile(fileName);
                            setupToolStripMenuItem.Enabled = false;
                            testToolStripMenuItem.Enabled = false;
                        }
                        break;
                    case JsonHelper.ConfigFileType.GameConfig:
                        if (jsonHelper.ValidateGameConfigStructure(fileName))
                        {
                            LoadGameConfigFile(fileName);
                            setupToolStripMenuItem.Enabled = true;
                            testToolStripMenuItem.Enabled = false;
                        }
                        break;
                    case JsonHelper.ConfigFileType.ListConfig:
                        if (jsonHelper.ValidateListConfigStructure(fileName))
                        {
                            LoadListConfigFile(fileName);
                            setupToolStripMenuItem.Enabled = false;
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
                    errorString.AppendLine(new string('-',48));
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
                    ResetEditFormItems();
                    tvBotData.Nodes[0].EnsureVisible(); // Scroll to Top
                }
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
            }
            systemActionsNode.Expand();
            TreeNode actionsNode = tvBotData.Nodes.Add("Actions");
            actionsNode.Name = "Actions";
            foreach (KeyValuePair<string, BotEngineClient.Action> item in gameConfig.Actions)
            {
                LoadActionTreeNode(actionsNode, item);
                cbPickActionAction.Items.Add(item.Key);
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
            loadedFileType = JsonHelper.ConfigFileType.DeviceConfig;
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
        /// Saves the data in tvBotData into the json file that it came from.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        }

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
            BOTConfig gameConfig = new BOTConfig
            {
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
                        BotEngineClient.Action newAction = new BotEngineClient.Action
                        {
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
                        BotEngineClient.Action newAction = new BotEngineClient.Action
                        {
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

        #region Create Files
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
                }
        }

        #endregion

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
        /// After selecting a node in the tvBotData, show the appropriate edit GroupBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvBotData_AfterSelect(object sender, TreeViewEventArgs e)
        {
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
                        case ValidCommandIds.WaitForThenClick:
                            if (!string.IsNullOrEmpty(commandCopy.ImageName))
                                cbImageNameWithWait.SelectedItem = commandCopy.ImageName;
                            else
                                cbImageNameWithWait.SelectedIndex = -1;
                            if (commandCopy.TimeOut != null)
                                tbTimeout.Text = commandCopy.TimeOut.ToString();
                            else
                                tbTimeout.Text = "";
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
                gbClick.Enabled = true;
                gbClick.Visible = true;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is BotEngineClient.ActionActivity actionActivity))
            {
                tbActionOverrideEnabled.Checked = actionActivity.ActionEnabled;
                tbActionOverrideName.Text = e.Node.Name;
                dtpActionOverrideLastRun.Value = actionActivity.LastRun;
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
                gbActionOverride.Enabled = true;
                gbActionOverride.Visible = true;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is List<XYCoords>))
            {
                tbListName.Text = ActiveTreeNode.Name;
                gbList.Enabled = true;
                gbList.Visible = true;
            }

            ChangePending = false;
            btnUpdate.Enabled = false;
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
                    ActiveTreeNode.Name = tbActionName.Text;
                    ActiveTreeNode.Text = tbActionName.Text;

                    botAction.ActionType = cbActionType.Text;
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
                                    SearchArea searchArea = new SearchArea
                                    {
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
                                    foreach(string item in lbImageNames.Items)
                                    {
                                        commandCopy.ImageNames.Add(item);
                                    }
                                }
                                commandCopy.TimeOut = int.Parse(tbImageNamesWait.Text);
                                break;
                            case ValidCommandIds.WaitFor:
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


        private void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDo: Implement Test of an Action Capability.
        }

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

        private void AllFields_TextChanged(object sender, EventArgs e)
        {
            SetChangePending();
        }

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

        private void BtnAddImageNames_Click(object sender, EventArgs e)
        {
            SetChangePending();
            lbImageNames.Items.Add(cbImageNamesForList.SelectedItem.ToString());
        }

        private void BtnRemoveImageNames_Click(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItems.Count > 0)
            {
                SetChangePending();
                lbImageNames.Items.RemoveAt(lbImageNames.SelectedIndex);
            }
        }

        private void LbImageNames_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItem != null)
                cbImageNamesForList.Text = lbImageNames.SelectedItem.ToString();
        }

        private void BtImageAreaAdd_Click(object sender, EventArgs e)
        {
            SetChangePending();
            lbImageAreaAreas.Items.Add(string.Format("({0}, {1}) - ({2}, {3})", tbImageAreasX.Text, tbImageAreasY.Text, tbImageAreasW.Text, tbImageAreasH.Text));
        }

        private void BtImageAreaRemove_Click(object sender, EventArgs e)
        {
            if (lbImageAreaAreas.SelectedItems.Count > 0)
            {
                SetChangePending();
                lbImageAreaAreas.Items.RemoveAt(lbImageAreaAreas.SelectedIndex);
            }
        }

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

        private void SetChangePending()
        {
            ChangePending = true;
            btnUpdate.Enabled = true;
        }

        private void BtnPasteFindText_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboard = Clipboard.GetText();
                try
                {
                    JsonDocumentOptions documentOptions = new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip
                    };
                    JsonNodeOptions nodeOptions = new JsonNodeOptions
                    {
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

        #region Add menu options
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

                TreeNode newNode = new TreeNode
                {
                    Text = "New Action",
                    Name = "New Action",
                    Tag = newAction
                };

                tvBotData.SelectedNode.Nodes.Add(newNode);
                tvBotData.SelectedNode = newNode;
            }
        }

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

                TreeNode newNode = new TreeNode
                {
                    Text = "New FindString",
                    Name = "New FindString",
                    Tag = newFindString
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
            else
            {
                List<XYCoords> newCoordinates = new List<XYCoords>();

                TreeNode newNode = new TreeNode
                {
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
                        TreeNode newNode = new TreeNode
                        {
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

        #endregion

        #region Insert menu options
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
                        TreeNode newNode = new TreeNode
                        {
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
                    TreeNode newNode = new TreeNode
                    {
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
                    TreeNode newNode = new TreeNode
                    {
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
                        TreeNode newNode = new TreeNode
                        {
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
                        TreeNode newNode = new TreeNode
                        {
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
                    TreeNode newNode = new TreeNode
                    {
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
                    TreeNode newNode = new TreeNode
                    {
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
        #endregion

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
        /// Delete the seleted node from tvBotData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = tvBotData.SelectedNode;
            if (currentNode != null)
            {
                tvBotData.Nodes.Remove(currentNode);
                UnsavedChanges = true;
                ChangePending = false;
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAboutBox about = new HelpAboutBox();
            about.ShowDialog();
        }
    }
}
