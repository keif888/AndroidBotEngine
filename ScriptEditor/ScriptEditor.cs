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

namespace ScriptEditor
{
    public partial class ScriptEditor : Form
    {
        private BOTConfig moeConfig;

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
            this.Size = new Size(800, 485);
            splitContainer1.SplitterDistance = 320;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                    TreeNode treeNode = new TreeNode();
                    treeNode.Name = item.Key;
                    treeNode.Tag = item.Value;
                    treeNode.Text = item.Key;
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

        private void tvBotData_AfterSelect(object sender, TreeViewEventArgs e)
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


            if ((e.Node.Tag != null) && (e.Node.Tag is Command))
            {
                Command commandCopy = (Command)e.Node.Tag;
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
                        tbImageNamesWait.Text = commandCopy.TimeOut.ToString();
                        gbImageNames.Enabled = true;
                        gbImageNames.Visible = true;
                        break;
                    case "findclick":
                    case "ifexists":
                    case "ifnotexists":
                        cbImageNameNoWait.SelectedItem = commandCopy.ImageName;
                        gbImageName.Enabled = true;
                        gbImageName.Visible = true;
                        break;
                    case "sleep":
                        tbDelay.Text = commandCopy.Delay.ToString();
                        gbSleep.Enabled = true;
                        gbSleep.Visible = true;
                        break;
                    case "findclickandwait":
                    case "waitfor":
                    case "waitforthenclick":
                        cbImageNameWithWait.SelectedItem = commandCopy.ImageName;
                        tbTimeout.Text = commandCopy.TimeOut.ToString();
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
                    case "clickwhennotfoundinarea":
                    case "loopcoordinates":
                    case "startgame":
                    case "stopgame":
                    default:
                        break;
                }
            }
            else if ((e.Node.Tag != null) && (e.Node.Tag is BotEngineClient.Action))
            {
                gbAction.Enabled = true;
                gbAction.Visible = true;
                BotEngineClient.Action actionCopy = (BotEngineClient.Action)e.Node.Tag;
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
            else if ((e.Node.Tag != null) && (e.Node.Tag is FindString))
            {
                gbFindText.Enabled = true;
                gbFindText.Visible = true;
                FindString findStringCopy = (FindString)e.Node.Tag;
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

    }
}
