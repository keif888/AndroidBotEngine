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
                switch (command.CommandId.ToLower())
                {
                    case "clickwhennotfoundinarea":
                    case "exit":
                    case "enterloopcoordinate":
                    case "restart":
                    case "startgame":
                    case "stopgame":
                        childText = string.Format("{0}", command.CommandId);
                        break;
                    case "runaction":
                        if (command.ActionName != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.ActionName);
                        break;
                    case "loopcoordinates":
                        childText = string.Format("{0} ({1})", command.CommandId, command.Coordinates);
                        break;
                    case "click":
                        if (command.Location != null)
                            childText = string.Format("{0} ({1},{2})", command.CommandId, command.Location.X, command.Location.Y);
                        break;
                    case "sleep":
                        childText = string.Format("{0} ({1})", command.CommandId, command.Delay);
                        break;
                    case "ifexists":
                    case "ifnotexists":
                    case "findclick":
                    case "findclickandwait":
                    case "loopuntilnotfound":
                    case "loopuntilfound":
                    case "waitfor":
                    case "waitforthenclick":
                        if (command.ImageName != null)
                            childText = string.Format("{0} ({1})", command.CommandId, command.ImageName);
                        else
                            childText = string.Format("{0} (list)", command.CommandId);
                        break;
                    case "drag":
                        if (command.Swipe != null)
                        {
                            childText = string.Format("{0} ({1}, {2}) - ({3}, {4})", command.CommandId, command.Swipe.X1, command.Swipe.Y1, command.Swipe.X2, command.Swipe.Y2);
                        }
                        break;
                    case "waitforchange":
                    case "waitfornochange":
                        if (command.ChangeDetectArea != null)
                        {
                            childText = string.Format("{0} ({1}, {2}) - ({3}, {4})", command.CommandId, command.ChangeDetectArea.X, command.ChangeDetectArea.Y, command.ChangeDetectArea.X + command.ChangeDetectArea.width, command.ChangeDetectArea.Y + command.ChangeDetectArea.height);
                        }
                        break;
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
                switch (commandCopy.CommandId.ToLower())
                {
                    case "click":
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
                    case "loopuntilfound":
                    case "loopuntilnotfound":
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
                    case "findclick":
                    case "ifexists":
                    case "ifnotexists":
                        if (commandCopy.ImageName != null)
                            cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                        else
                            cbImageNameNoWait.SelectedIndex = -1;
                        gbImageName.Enabled = true;
                        gbImageName.Visible = true;
                        break;
                    case "sleep":
                        if (commandCopy.Delay != null)
                            tbDelay.Text = commandCopy.Delay.ToString();
                        else
                            tbDelay.Text = "";
                        gbSleep.Enabled = true;
                        gbSleep.Visible = true;
                        break;
                    case "findclickandwait":
                    case "waitfor":
                    case "waitforthenclick":
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
                    case "waitforchange":
                    case "waitfornochange":
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
                        gbWFNC.Visible = true;
                        break;
                    case "drag":
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
                    case "enterloopcoordinate":
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
                    case "runaction":
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
                    case "startgame":
                    case "stopgame":
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
                    case "clickwhennotfoundinarea":
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
                    case "loopcoordinates":
                        if (commandCopy.Coordinates != null)
                            tbListName.Text = commandCopy.Coordinates;
                        else
                            tbListName.Text = string.Empty;
                        gbList.Enabled = true;
                        gbList.Visible = true;
                        break;
                    case "exit":
                    case "restart":
                        // Nothing to do here, as there is no UI element for these.
                        break;
                    default:
                        MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                        break;
                }
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is BotEngineClient.Action action))
            {
                gbAction.Enabled = true;
                gbAction.Visible = true;
                BotEngineClient.Action actionCopy = action;
                tbActionName.Text = e.Node.Name;
                cbActionType.Text = actionCopy.ActionType;
                switch (actionCopy.ActionType.ToLower())
                {
                    case "scheduled":
                        dtpActionTimeOfDay.Enabled = false;
                        tbActionFrequency.Enabled = true;
                        break;
                    case "daily":
                        dtpActionTimeOfDay.Enabled = true;
                        tbActionFrequency.Enabled = false;
                        break;
                    default:
                        dtpActionTimeOfDay.Enabled = false;
                        tbActionFrequency.Enabled = false;
                        break;
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
                    switch (botAction.ActionType.ToLower())
                    {
                        case "scheduled":
                            botAction.Frequency = int.Parse(tbActionFrequency.Text);
                            botAction.DailyScheduledTime = null;
                            break;
                        case "daily":
                            botAction.Frequency = null;
                            botAction.DailyScheduledTime = dtpActionTimeOfDay.Value;
                            break;
                        default:
                            botAction.Frequency = null;
                            botAction.DailyScheduledTime = null;
                            break;
                    }
                }
                else if (selectedTag is Command commandCopy)
                {
                    switch (commandCopy.CommandId.ToLower())
                    {
                        case "click":
                            if (commandCopy.Location == null)
                            {
                                commandCopy.Location = new XYCoords();
                            }
                            commandCopy.Location.X = int.Parse(tbPointX.Text);
                            commandCopy.Location.Y = int.Parse(tbPointY.Text);
                            ActiveTreeNode.Text = string.Format("{0} ({1},{2})", commandCopy.CommandId, commandCopy.Location.X, commandCopy.Location.Y);
                            break;
                        case "loopuntilfound":
                        case "loopuntilnotfound":
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
                        case "findclick":
                        case "ifexists":
                        case "ifnotexists":
                            commandCopy.ImageName = (string)cbImageNameNoWait.SelectedItem;
                            break;
                        case "sleep":
                            commandCopy.Delay = int.Parse(tbDelay.Text);
                            break;
                        case "findclickandwait":
                        case "waitfor":
                        case "waitforthenclick":
                            commandCopy.ImageName = (string)cbImageNameWithWait.SelectedItem;
                            commandCopy.TimeOut = int.Parse(tbTimeout.Text);
                            break;
                        case "waitforchange":
                        case "waitfornochange":
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
                        case "drag":
                            if (commandCopy.Swipe == null)
                            {
                                commandCopy.Swipe = new SwipeCoords();
                            }
                            commandCopy.Swipe.X1 = int.Parse(tbDragX1.Text);
                            commandCopy.Swipe.Y1 = int.Parse(tbDragY1.Text);
                            commandCopy.Swipe.X2 = int.Parse(tbDragX2.Text);
                            commandCopy.Swipe.Y2 = int.Parse(tbDragY2.Text);
                            break;
                        case "enterloopcoordinate":
                            if (rbLoopCoordX.Checked)
                            { 
                                commandCopy.Value = "X"; 
                            }
                            else
                            {
                                commandCopy.Value = "Y";
                            }
                            break;
                        case "runaction":
                            commandCopy.ActionName = cbPickActionAction.Text;
                            break;
                        case "clickwhennotfoundinarea":
                        //ToDo: Bind clickwhennotfoundinarea
                        case "loopcoordinates":
                            //ToDo: Bind LoopCoordinates
                            break;
                        case "startgame":
                        case "stopgame":
                            commandCopy.Value = tbAppNameAppId.Text;
                            commandCopy.TimeOut = int.Parse(tbAppNameTimeout.Text);
                            break;
                        default:
                            MessageBox.Show(string.Format("CommandId {0} hasn't been implmented in Editor", commandCopy.CommandId));
                            break;
                    }
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
            //ToDo: Adjust Enabled fields, and update flags.
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
    }
}
