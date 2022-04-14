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
        private BOTConfig moeConfig;
        private bool ChangePending;
        private AdbServer? server;

        public ScriptEditor()
        {
            InitializeComponent();
            moeConfig = new BOTConfig();
            gbClick.Top = 5;
            gbDrag.Top = 5;
            gbImageName.Top = 5;
            gbImageNameAndWait.Top = 5;
            gbImageNames.Top = 5;
            gbLoopCoordinate.Top = 5;
            gbSleep.Top = 5;
            gbClick.Left = 5;
            gbDrag.Left = 5;
            gbImageName.Left = 5;
            gbImageNameAndWait.Left = 5;
            gbImageNames.Left = 5;
            gbLoopCoordinate.Left = 5;
            gbSleep.Left = 5;
            gbAction.Top = 5;
            gbAction.Left = 5;
            gbFindText.Top = 5;
            gbFindText.Left = 5;
            gbWFNC.Top = 5;
            gbWFNC.Left = 5;
            gbAppControl.Top = 5;
            gbAppControl.Left = 5;
            gbPickAction.Top = 5;
            gbPickAction.Left = 5;
            gbAppName.Top = 5;
            gbAppName.Left = 5;
            this.Size = new Size(800, 485);
            splitContainer1.SplitterDistance = 320;
            ChangePending = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                string jsonString = File.ReadAllText(fileName);
                try
                {
                    moeConfig = JsonSerializer.Deserialize<BOTConfig>(jsonString, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip })!;
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
                foreach (KeyValuePair<string, BotEngineClient.FindString> item in moeConfig.findStrings)
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
                foreach (KeyValuePair<string, BotEngineClient.Action> item in moeConfig.systemActions)
                {
                    LoadActionTreeNode(systemActionsNode, item);
                    cbPickActionAction.Items.Add(item.Key);
                }
                TreeNode actionsNode = tvBotData.Nodes.Add("actions");
                foreach (KeyValuePair<string, BotEngineClient.Action> item in moeConfig.actions)
                {
                    LoadActionTreeNode(actionsNode, item);
                    cbPickActionAction.Items.Add(item.Key);
                }

                tvBotData.ResumeLayout();
                ChangePending = false;
                saveToolStripMenuItem.Enabled = false;
            }
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
            gbDrag.Enabled = false;
            gbImageName.Enabled = false;
            gbImageNameAndWait.Enabled = false;
            gbImageNames.Enabled = false;
            gbLoopCoordinate.Enabled = false;
            gbSleep.Enabled = false;
            gbClick.Visible = false;
            gbDrag.Visible = false;
            gbImageName.Visible = false;
            gbImageNameAndWait.Visible = false;
            gbImageNames.Visible = false;
            gbLoopCoordinate.Visible = false;
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
                    case "loopcoordinates":
                    default:
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
                        dtpActionTimeOfDay.Text = actionCopy.DailyScheduledTime.ToString();
                    else
                        dtpActionTimeOfDay.Text = "2001-01-01 10:00am";
                }
                else
                {
                    dtpActionTimeOfDay.Text = "2001-01-01 10:00am";
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
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ChangePending)
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
            var selectedTag = tvBotData.SelectedNode.Tag;
            if (selectedTag != null)
            {
                ChangePending = true;
                saveToolStripMenuItem.Enabled = true;
                if (selectedTag is FindString findTag)
                {
                    tvBotData.SelectedNode.Name = tbFindTextName.Text;
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
                    tvBotData.SelectedNode.Name = tbActionName.Text;

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
                        case "loopcoordinates":
                        case "startgame":
                        case "stopgame":
                        default:
                            break;
                    }
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
    }
}
