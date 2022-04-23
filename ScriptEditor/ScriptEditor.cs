using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using BotEngineClient;
using SharpAdbClient;
using static BotEngineClient.BotEngine;

namespace ScriptEditor
{
    public partial class ScriptEditor : Form
    {
        private BOTConfig gameConfig;
        private BOTDeviceConfig deviceConfig;
        private BOTListConfig listConfig;
        private bool ChangePending;
        private bool UnsavedChanges;
        private AdbServer? server;
        private JsonHelper.ConfigFileType loadedFileType;
        private string JsonFileName;
        private TreeNode ActiveTreeNode;

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

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                JsonHelper jsonHelper = new JsonHelper();
                switch (jsonHelper.getFileType(fileName))
                {
                    case JsonHelper.ConfigFileType.DeviceConfig:
                        if (jsonHelper.ValidateDeviceConfigStructure(fileName))
                        {
                            LoadDeviceConfigFile(fileName);
                        }
                        break;
                    case JsonHelper.ConfigFileType.GameConfig:
                        if (jsonHelper.ValidateGameConfigStructure(fileName))
                        {
                            LoadGameConfigFile(fileName);
                        }
                        break;
                    case JsonHelper.ConfigFileType.ListConfig:
                        if (jsonHelper.ValidateListConfigStructure(fileName))
                        {
                            LoadListConfigFile(fileName);
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
                }
            }
        }

        private void LoadGameConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                gameConfig = JsonSerializer.Deserialize<BOTConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
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
            TreeNode findStringsNode = tvBotData.Nodes.Add("findStrings");
            findStringsNode.Name = "findStrings";
            foreach (KeyValuePair<string, BotEngineClient.FindString> item in gameConfig.findStrings)
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
            }
            TreeNode systemActionsNode = tvBotData.Nodes.Add("systemActions");
            systemActionsNode.Name = "systemActions";
            foreach (KeyValuePair<string, BotEngineClient.Action> item in gameConfig.systemActions)
            {
                LoadActionTreeNode(systemActionsNode, item);
                cbPickActionAction.Items.Add(item.Key);
            }
            TreeNode actionsNode = tvBotData.Nodes.Add("actions");
            actionsNode.Name = "actions";
            foreach (KeyValuePair<string, BotEngineClient.Action> item in gameConfig.actions)
            {
                LoadActionTreeNode(actionsNode, item);
                cbPickActionAction.Items.Add(item.Key);
            }

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.GameConfig;
        }

        private void LoadDeviceConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                deviceConfig = JsonSerializer.Deserialize<BOTDeviceConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
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

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.DeviceConfig;
        }

        private void LoadListConfigFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            try
            {
                listConfig = JsonSerializer.Deserialize<BOTListConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
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

            tvBotData.ResumeLayout();
            ChangePending = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            loadedFileType = JsonHelper.ConfigFileType.DeviceConfig;
        }

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

        private void LoadActionTreeNode(TreeNode parent, List<BotEngineClient.Command> commands)
        {
            foreach (BotEngineClient.Command command in commands)
            {
                string childText = string.Format("{0}", command.CommandId);
                childText = command.CommandId;
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
                                childText = string.Format("{0} ({1}, {2}) - ({3}, {4})", command.CommandId, command.ChangeDetectArea.X, command.ChangeDetectArea.Y, command.ChangeDetectArea.X + command.ChangeDetectArea.width, command.ChangeDetectArea.Y + command.ChangeDetectArea.height);
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
                    Name = command.CommandNumber.ToString(),
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

        private void TvBotData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            gbClick.Enabled = false;
            gbClick.Visible = false;
            gbDrag.Enabled = false;
            gbDrag.Visible = false;
            gbImageName.Enabled = false;
            gbImageName.Visible = false;
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

            if (e.Node != null)
                ActiveTreeNode = e.Node;

            if ((e.Node.Tag != null) && (e.Node.Tag is Command command))
            {
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
                                    lbImageAreaAreas.Items.Add(string.Format("({0}, {1}) - ({2}, {3})", areaItem.X, areaItem.Y, areaItem.X + areaItem.width, areaItem.Y + areaItem.height));
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
                            // ToDo: Add IgnoreMissing
                            if (commandCopy.ImageName != null)
                                cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                            else
                                cbImageNameNoWait.SelectedIndex = -1;
                            gbImageName.Enabled = true;
                            gbImageName.Visible = true;
                            break;
                        case ValidCommandIds.IfExists:
                        case ValidCommandIds.IfNotExists:
                            if (commandCopy.ImageName != null)
                                cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                            else
                                cbImageNameNoWait.SelectedIndex = -1;
                            gbImageName.Enabled = true;
                            gbImageName.Visible = true; break;
                        case ValidCommandIds.LoopCoordinates:
                            if (commandCopy.Coordinates != null)
                                tbListName.Text = commandCopy.Coordinates;
                            else
                                tbListName.Text = string.Empty;
                            gbList.Enabled = true;
                            gbList.Visible = true;
                            break;
                        case ValidCommandIds.FindClickAndWait:
                        case ValidCommandIds.LoopUntilFound:
                        case ValidCommandIds.LoopUntilNotFound:
                            lbImageNames.Items.Clear();
                            if (commandCopy.ImageNames != null)
                                foreach (string item in commandCopy.ImageNames)
                                {
                                    lbImageNames.Items.Add(item);
                                }
                            if (commandCopy.ImageName != null)
                                lbImageNames.Items.Add(commandCopy.ImageName);
                            if (commandCopy.TimeOut != null)
                                tbImageNamesWait.Text = commandCopy.TimeOut.ToString();
                            else
                                tbImageNamesWait.Text = "";
                            gbImageNames.Enabled = true;
                            gbImageNames.Visible = true;
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
                            if (commandCopy.ImageName != null)
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
                                tbWFNCX2.Text = (commandCopy.ChangeDetectArea.X + commandCopy.ChangeDetectArea.width).ToString();
                                tbWFNCY2.Text = (commandCopy.ChangeDetectArea.Y + commandCopy.ChangeDetectArea.height).ToString();
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
                            gbWFNC.Visible = true; break;
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
                if (e.Node.Parent.Name == "systemActions")
                    cbActionType.Enabled = false;
                else
                    cbActionType.Enabled = true;
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is FindString findString))
            {
                gbFindText.Enabled = true;
                gbFindText.Visible = true;
                FindString findStringCopy = findString;
                tbFindTextBackTolerance.Text = findStringCopy.backgroundTolerance.ToString();
                tbFindTextTextTolerance.Text = findStringCopy.textTolerance.ToString();
                tbFindTextName.Text = e.Node.Name;
                tbFindTextSearch.Text = findStringCopy.findString;
                tbFindTextSearchX1.Text = findStringCopy.searchArea.X.ToString();
                tbFindTextSearchY1.Text = findStringCopy.searchArea.Y.ToString();
                tbFindTextSearchX2.Text = (findStringCopy.searchArea.X + findStringCopy.searchArea.width).ToString();
                tbFindTextSearchY2.Text = (findStringCopy.searchArea.Y + findStringCopy.searchArea.height).ToString();
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

            ChangePending = false;
            btnUpdate.Enabled = false;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                if (MessageBox.Show("You have pending changes, exit?", "Pending Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var selectedTag = ActiveTreeNode.Tag;
            if (selectedTag != null)
            {
                UnsavedChanges = true;
                ChangePending = false;
                btnUpdate.Enabled = false;
                saveToolStripMenuItem.Enabled = true;
                if (selectedTag is FindString findTag)
                {
                    ActiveTreeNode.Name = tbFindTextName.Text;
                    findTag.backgroundTolerance = float.Parse(tbFindTextBackTolerance.Text);
                    findTag.textTolerance = float.Parse(tbFindTextTextTolerance.Text);
                    findTag.findString = tbFindTextSearch.Text;
                    findTag.searchArea.X = int.Parse(tbFindTextSearchX1.Text);
                    findTag.searchArea.Y = int.Parse(tbFindTextSearchY1.Text);
                    findTag.searchArea.width = int.Parse(tbFindTextSearchX2.Text) - findTag.searchArea.X;
                    findTag.searchArea.height = int.Parse(tbFindTextSearchY2.Text) - findTag.searchArea.Y;
                }
                else if (selectedTag is BotEngineClient.Action botAction)
                {
                    ActiveTreeNode.Name = tbActionName.Text;

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
                                botAction.Frequency = int.Parse(tbActionFrequency.Text);
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
                        switch (validCommandIds)
                        {
                            case ValidCommandIds.Click:
                                if (commandCopy.Location == null)
                                {
                                    commandCopy.Location = new XYCoords();
                                }
                                commandCopy.Location.X = int.Parse(tbPointX.Text);
                                commandCopy.Location.Y = int.Parse(tbPointY.Text);
                                ActiveTreeNode.Text = string.Format("{0} ({1},{2})", commandCopy.CommandId, commandCopy.Location.X, commandCopy.Location.Y);
                                break;
                            case ValidCommandIds.ClickWhenNotFoundInArea:
                                commandCopy.ImageName = cbImageAreasImage.Text;
                                commandCopy.Areas.Clear();
                                char[] delimiters = { ',', '-' };
                                foreach (string item in lbImageAreaAreas.Items)
                                {
                                    string[] values = item.Replace("(", "").Replace(")", "").Replace(" ", "").Split(delimiters);
                                    SearchArea searchArea = new SearchArea();
                                    searchArea.X = int.Parse(values[0]);
                                    searchArea.Y = int.Parse(values[1]);
                                    searchArea.width = int.Parse(values[2]) - searchArea.X;
                                    searchArea.height = int.Parse(values[3]) - searchArea.Y;
                                    commandCopy.Areas.Add(searchArea);
                                }
                                break;
                            case ValidCommandIds.Drag:
                                if (commandCopy.Swipe == null)
                                {
                                    commandCopy.Swipe = new SwipeCoords();
                                }
                                commandCopy.Swipe.X1 = int.Parse(tbDragX1.Text);
                                commandCopy.Swipe.Y1 = int.Parse(tbDragY1.Text);
                                commandCopy.Swipe.X2 = int.Parse(tbDragX2.Text);
                                commandCopy.Swipe.Y2 = int.Parse(tbDragY2.Text);
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
                                // ToDo: Add IgnoreMissing
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
                                if (lbImageNames.Items.Count == 0)
                                {
                                    commandCopy.ImageName = null;
                                    commandCopy.ImageNames = null;
                                }
                                else if (lbImageNames.Items.Count == 1)
                                {
                                    commandCopy.ImageName = (string)lbImageNames.Items[0];
                                    commandCopy.ImageNames = null;
                                    ActiveTreeNode.Text = string.Format("{0} ({1})", validCommandIds, commandCopy.ImageName);
                                }
                                else
                                {
                                    commandCopy.ImageName = null;
                                    foreach (string item in lbImageNames.Items)
                                    {
                                        commandCopy.ImageNames.Add(item);
                                    }
                                    ActiveTreeNode.Text = string.Format("{0} (list)", validCommandIds);
                                }
                                commandCopy.TimeOut = int.Parse(tbImageNamesWait.Text);
                                break;
                            case ValidCommandIds.Restart:
                                break;
                            case ValidCommandIds.RunAction:
                                commandCopy.ActionName = cbPickActionAction.Text;
                                ActiveTreeNode.Text = string.Format("{0} ({1})", validCommandIds, commandCopy.ActionName);
                                break;
                            case ValidCommandIds.Sleep:
                                commandCopy.Delay = int.Parse(tbDelay.Text);
                                ActiveTreeNode.Text = string.Format("{0} ({1})", validCommandIds, commandCopy.Delay);
                                break;
                            case ValidCommandIds.StartGame:
                            case ValidCommandIds.StopGame:
                                commandCopy.Value = tbAppNameAppId.Text;
                                commandCopy.TimeOut = int.Parse(tbAppNameTimeout.Text);
                                break;
                            case ValidCommandIds.FindClickAndWait:
                            case ValidCommandIds.WaitFor:
                            case ValidCommandIds.WaitForThenClick:
                                commandCopy.ImageName = (string)cbImageNameWithWait.SelectedItem;
                                commandCopy.TimeOut = int.Parse(tbTimeout.Text);
                                ActiveTreeNode.Text = string.Format("{0} ({1})", validCommandIds, commandCopy.ImageName);
                                break;
                            case ValidCommandIds.WaitForChange:
                            case ValidCommandIds.WaitForNoChange:
                                commandCopy.TimeOut = int.Parse(tbWFNCWait.Text);
                                if (commandCopy.ChangeDetectArea == null)
                                {
                                    commandCopy.ChangeDetectArea = new SearchArea();
                                }
                                commandCopy.ChangeDetectArea.X = int.Parse(tbWFNCX1.Text);
                                commandCopy.ChangeDetectArea.Y = int.Parse(tbWFNCY1.Text);
                                commandCopy.ChangeDetectArea.width = int.Parse(tbWFNCX2.Text) - commandCopy.ChangeDetectArea.X;
                                commandCopy.ChangeDetectArea.height = int.Parse(tbWFNCY2.Text) - commandCopy.ChangeDetectArea.Y;
                                commandCopy.ChangeDetectDifference = (float)nudWFNCDetectPercent.Value / 100.0f;
                                break;
                            default:
                                MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                                break;
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
                        actionActivity.Frequency = int.Parse(tbActionOverrideFrequency.Text);
                    if (dtptbActionOverrideTimeOfDay.Checked)
                        actionActivity.DailyScheduledTime = dtptbActionOverrideTimeOfDay.Value;
                    else
                        actionActivity.DailyScheduledTime = null;
                    ActiveTreeNode.Text = string.Format("{0} - {1}", tbActionOverrideName.Text, actionActivity.ActionEnabled ? "Enabled" : "Disabled");
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (loadedFileType)
            {
                case JsonHelper.ConfigFileType.GameConfig:
                    SaveGameConfig();
                    //ToDo: Save GameConfig file
                    break;
                case JsonHelper.ConfigFileType.ListConfig:
                    // ToDo: Save ListConfig file
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
        /// Saves a Game Config file from the content saved within the tvBotData tree view and it's Tags
        /// </summary>
        private void SaveGameConfig()
        {
            BOTConfig gameConfig = new BOTConfig();
            gameConfig.FileId = loadedFileType.ToString();
            gameConfig.findStrings = new Dictionary<string, FindString>();
            gameConfig.systemActions = new Dictionary<string, BotEngineClient.Action>();
            gameConfig.actions = new Dictionary<string, BotEngineClient.Action>();
            foreach (TreeNode parent in tvBotData.Nodes)
            {
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "findStrings")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        gameConfig.findStrings.Add(child.Name, (FindString)child.Tag);
                    }
                }
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "systemActions")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        BotEngineClient.Action sourceAction = (BotEngineClient.Action)child.Tag;
                        BotEngineClient.Action newAction = new BotEngineClient.Action();
                        newAction.ActionType = sourceAction.ActionType;
                        newAction.AfterAction = sourceAction.AfterAction;
                        newAction.BeforeAction = sourceAction.BeforeAction;
                        if (child.Nodes.Count != 0)
                        {
                            int commandNumber = 10;
                            List<Command> childCommands = new List<Command>();
                            LoadCommandList(childCommands, child, ref commandNumber);
                            newAction.Commands = childCommands;
                        }
                        newAction.DailyScheduledTime = sourceAction.DailyScheduledTime;
                        newAction.Frequency = sourceAction.Frequency;
                        gameConfig.systemActions.Add(child.Name, newAction);
                    }
                }
                if (parent.Tag is null && parent.Nodes.Count > 0 && parent.Name == "actions")
                {
                    foreach (TreeNode child in parent.Nodes)
                    {
                        BotEngineClient.Action sourceAction = (BotEngineClient.Action)child.Tag;
                        BotEngineClient.Action newAction = new BotEngineClient.Action();
                        newAction.ActionType = sourceAction.ActionType;
                        newAction.AfterAction = sourceAction.AfterAction;
                        newAction.BeforeAction = sourceAction.BeforeAction;
                        if (child.Nodes.Count != 0)
                        {
                            int commandNumber = 10;
                            List<Command> childCommands = new List<Command>();
                            LoadCommandList(childCommands, child, ref commandNumber);
                            newAction.Commands = childCommands;
                        }
                        newAction.DailyScheduledTime = sourceAction.DailyScheduledTime;
                        newAction.Frequency = sourceAction.Frequency;
                        gameConfig.actions.Add(child.Name, newAction);
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

        private void LoadCommandList(List<Command> commands, TreeNode parent, ref int commandNumber)
        {
            foreach (TreeNode childNode in parent.Nodes)
            {
                Command childCommand = (Command) childNode.Tag;
                Command newCommand = new Command(childCommand.CommandId);
                newCommand.ActionName = childCommand.ActionName;
                newCommand.Areas = childCommand.Areas;
                newCommand.ChangeDetectArea = childCommand.ChangeDetectArea;
                newCommand.ChangeDetectDifference = childCommand.ChangeDetectDifference;
                newCommand.CommandNumber = commandNumber;
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
            BOTDeviceConfig deviceConfig = new BOTDeviceConfig();
            deviceConfig.FileId = loadedFileType.ToString();
            deviceConfig.LastActionTaken = new Dictionary<string, ActionActivity>();
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
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Unable to save file with {0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDo: Implement Test of an Action Capability.
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextEdit fte = new FindTextEdit();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                string searchText = fte.SearchText;
                Rectangle searchArea = fte.SearchRectangle;
            }
        }

        private void AllFields_TextChanged(object sender, EventArgs e)
        {
            setChangePending();
        }

        private void tvBotData_BeforeSelect(object sender, TreeViewCancelEventArgs e)
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

        private void cbActionType_TextChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse(cbActionType.Text, true, out ValidActionType validActionType))
            {
                switch (validActionType)
                {
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
            setChangePending();
        }

        private void btnAddImageNames_Click(object sender, EventArgs e)
        {
            setChangePending();
            lbImageNames.Items.Add(cbImageNamesForList.SelectedItem.ToString());
        }

        private void btnRemoveImageNames_Click(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItems.Count > 0)
            {
                setChangePending();
                lbImageNames.Items.RemoveAt(lbImageNames.SelectedIndex);
            }
        }

        private void lbImageNames_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lbImageNames.SelectedItem != null)
                cbImageNamesForList.Text = lbImageNames.SelectedItem.ToString();
        }

        private void btImageAreaAdd_Click(object sender, EventArgs e)
        {
            setChangePending();
            lbImageAreaAreas.Items.Add(string.Format("({0}, {1}) - ({2}, {3})", tbImageAreasX.Text, tbImageAreasY.Text, tbImageAreasW.Text, tbImageAreasH.Text));
        }

        private void btImageAreaRemove_Click(object sender, EventArgs e)
        {
            if (lbImageAreaAreas.SelectedItems.Count > 0)
            {
                setChangePending();
                lbImageAreaAreas.Items.RemoveAt(lbImageAreaAreas.SelectedIndex);
            }
        }

        private void lbImageAreaAreas_SelectedIndexChanged(object sender, EventArgs e)
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

        private void setChangePending()
        {
            ChangePending = true;
            btnUpdate.Enabled = true;
        }

        private void btnPastFindText_Click(object sender, EventArgs e)
        {
            // ToDo: Implement FindText Paste.
        }
    }
}
