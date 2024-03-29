﻿
namespace ScriptEditor
{
    partial class ScriptEditor
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Ov1",
            "1500"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFindStringtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCoordinatestoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.addCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvBotData = new System.Windows.Forms.TreeView();
            this.gbEnterText = new System.Windows.Forms.GroupBox();
            this.tbEnterTextText = new System.Windows.Forms.TextBox();
            this.tbEnterTextOverideId = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.gbLoops = new System.Windows.Forms.GroupBox();
            this.tbLoopsOverrideId = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.tbLoopsCounter = new System.Windows.Forms.MaskedTextBox();
            this.gbList = new System.Windows.Forms.GroupBox();
            this.tbListName = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.gbImageArea = new System.Windows.Forms.GroupBox();
            this.btImageAreaRemove = new System.Windows.Forms.Button();
            this.btImageAreaAdd = new System.Windows.Forms.Button();
            this.tbImageAreasX = new System.Windows.Forms.MaskedTextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.tbImageAreasH = new System.Windows.Forms.MaskedTextBox();
            this.tbImageAreasY = new System.Windows.Forms.MaskedTextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tbImageAreasW = new System.Windows.Forms.MaskedTextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.lbImageAreaAreas = new System.Windows.Forms.ListBox();
            this.label30 = new System.Windows.Forms.Label();
            this.cbImageAreasImage = new System.Windows.Forms.ComboBox();
            this.gbActionOverride = new System.Windows.Forms.GroupBox();
            this.btActionOverridesEdit = new System.Windows.Forms.Button();
            this.lvActionOverridesOverride = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tbActionOverrideValue = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.btActionOverridesRemove = new System.Windows.Forms.Button();
            this.btActionOverridesAdd = new System.Windows.Forms.Button();
            this.tbActionOverrideOverrideId = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.dtpActionOverrideLastRun = new System.Windows.Forms.DateTimePicker();
            this.tbActionOverrideEnabled = new System.Windows.Forms.CheckBox();
            this.dtptbActionOverrideTimeOfDay = new System.Windows.Forms.DateTimePicker();
            this.label26 = new System.Windows.Forms.Label();
            this.tbActionOverrideName = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tbActionOverrideFrequency = new System.Windows.Forms.MaskedTextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.gbAppName = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.tbAppNameTimeout = new System.Windows.Forms.MaskedTextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbAppNameAppId = new System.Windows.Forms.TextBox();
            this.gbPickAction = new System.Windows.Forms.GroupBox();
            this.cbPickActionAction = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.gbAppControl = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbAppControlWait = new System.Windows.Forms.MaskedTextBox();
            this.tbAppControlName = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.gbWFNC = new System.Windows.Forms.GroupBox();
            this.nudWFNCDetectPercent = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.tbWFNCWait = new System.Windows.Forms.MaskedTextBox();
            this.tbWFNCX1 = new System.Windows.Forms.MaskedTextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbWFNCY2 = new System.Windows.Forms.MaskedTextBox();
            this.tbWFNCY1 = new System.Windows.Forms.MaskedTextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tbWFNCX2 = new System.Windows.Forms.MaskedTextBox();
            this.gbFindText = new System.Windows.Forms.GroupBox();
            this.btnFindTextGenerate = new System.Windows.Forms.Button();
            this.btnPastFindText = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.tbFindTextBackTolerance = new System.Windows.Forms.MaskedTextBox();
            this.tbFindTextTextTolerance = new System.Windows.Forms.MaskedTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbFindTextSearchX1 = new System.Windows.Forms.MaskedTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbFindTextSearchY2 = new System.Windows.Forms.MaskedTextBox();
            this.tbFindTextSearchY1 = new System.Windows.Forms.MaskedTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbFindTextSearchX2 = new System.Windows.Forms.MaskedTextBox();
            this.tbFindTextSearch = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbFindTextName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.cbActionAfter = new System.Windows.Forms.ComboBox();
            this.label37 = new System.Windows.Forms.Label();
            this.cbActionBefore = new System.Windows.Forms.ComboBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.cbActionType = new System.Windows.Forms.ComboBox();
            this.dtpActionTimeOfDay = new System.Windows.Forms.DateTimePicker();
            this.label21 = new System.Windows.Forms.Label();
            this.tbActionName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbActionFrequency = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.gbSleep = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDelay = new System.Windows.Forms.MaskedTextBox();
            this.gbImageNames = new System.Windows.Forms.GroupBox();
            this.cbImageNamesMissingOk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbImageNamesWait = new System.Windows.Forms.MaskedTextBox();
            this.btnRemoveImageNames = new System.Windows.Forms.Button();
            this.btnAddImageNames = new System.Windows.Forms.Button();
            this.cbImageNamesForList = new System.Windows.Forms.ComboBox();
            this.lbImageNames = new System.Windows.Forms.ListBox();
            this.gbImageNameAndWait = new System.Windows.Forms.GroupBox();
            this.cbImageNameMissingOk = new System.Windows.Forms.CheckBox();
            this.cbImageNameWithWait = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTimeout = new System.Windows.Forms.MaskedTextBox();
            this.gbImageName = new System.Windows.Forms.GroupBox();
            this.cbIgnoreMissing = new System.Windows.Forms.CheckBox();
            this.cbImageNameNoWait = new System.Windows.Forms.ComboBox();
            this.gbLoopCoordinate = new System.Windows.Forms.GroupBox();
            this.rbLoopCoordY = new System.Windows.Forms.RadioButton();
            this.rbLoopCoordX = new System.Windows.Forms.RadioButton();
            this.gbDrag = new System.Windows.Forms.GroupBox();
            this.label35 = new System.Windows.Forms.Label();
            this.tbDragTime = new System.Windows.Forms.MaskedTextBox();
            this.tbDragX1 = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDragY2 = new System.Windows.Forms.MaskedTextBox();
            this.tbDragY1 = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDragX2 = new System.Windows.Forms.MaskedTextBox();
            this.gbClick = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPointX = new System.Windows.Forms.MaskedTextBox();
            this.tbPointY = new System.Windows.Forms.MaskedTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.scriptEditorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmsFindString = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFindStringToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAction = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addActionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addCommandToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pasteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsBasicCommand = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertAboveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBelowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsContainingCommand = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCommandToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.insertAboveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBelowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbEnterText.SuspendLayout();
            this.gbLoops.SuspendLayout();
            this.gbList.SuspendLayout();
            this.gbImageArea.SuspendLayout();
            this.gbActionOverride.SuspendLayout();
            this.gbAppName.SuspendLayout();
            this.gbPickAction.SuspendLayout();
            this.gbAppControl.SuspendLayout();
            this.gbWFNC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWFNCDetectPercent)).BeginInit();
            this.gbFindText.SuspendLayout();
            this.gbAction.SuspendLayout();
            this.gbSleep.SuspendLayout();
            this.gbImageNames.SuspendLayout();
            this.gbImageNameAndWait.SuspendLayout();
            this.gbImageName.SuspendLayout();
            this.gbLoopCoordinate.SuspendLayout();
            this.gbDrag.SuspendLayout();
            this.gbClick.SuspendLayout();
            this.cmsFindString.SuspendLayout();
            this.cmsAction.SuspendLayout();
            this.cmsBasicCommand.SuspendLayout();
            this.cmsContainingCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.textToolStripMenuItem,
            this.testToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1689, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFindStringtoolStripMenuItem,
            this.addActionToolStripMenuItem,
            this.addCoordinatestoolStripMenuItem,
            this.toolStripMenuItem5,
            this.addCommandToolStripMenuItem,
            this.aboveToolStripMenuItem,
            this.belowToolStripMenuItem,
            this.toolStripMenuItem3,
            this.moveToolStripMenuItem,
            this.upToolStripMenuItem,
            this.downToolStripMenuItem,
            this.toolStripMenuItem4,
            this.deleteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // addFindStringtoolStripMenuItem
            // 
            this.addFindStringtoolStripMenuItem.Enabled = false;
            this.addFindStringtoolStripMenuItem.Name = "addFindStringtoolStripMenuItem";
            this.addFindStringtoolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addFindStringtoolStripMenuItem.Text = "Add &FindString";
            this.addFindStringtoolStripMenuItem.Click += new System.EventHandler(this.AddFindStringtoolStripMenuItem_Click);
            // 
            // addActionToolStripMenuItem
            // 
            this.addActionToolStripMenuItem.Enabled = false;
            this.addActionToolStripMenuItem.Name = "addActionToolStripMenuItem";
            this.addActionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addActionToolStripMenuItem.Text = "Add Ac&tion";
            this.addActionToolStripMenuItem.Click += new System.EventHandler(this.AddActionToolStripMenuItem_Click);
            // 
            // addCoordinatestoolStripMenuItem
            // 
            this.addCoordinatestoolStripMenuItem.Enabled = false;
            this.addCoordinatestoolStripMenuItem.Name = "addCoordinatestoolStripMenuItem";
            this.addCoordinatestoolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addCoordinatestoolStripMenuItem.Text = "Add &Coordinates";
            this.addCoordinatestoolStripMenuItem.Click += new System.EventHandler(this.AddCoordinatestoolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(160, 6);
            // 
            // addCommandToolStripMenuItem
            // 
            this.addCommandToolStripMenuItem.Enabled = false;
            this.addCommandToolStripMenuItem.Name = "addCommandToolStripMenuItem";
            this.addCommandToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addCommandToolStripMenuItem.Text = "Add C&ommand";
            this.addCommandToolStripMenuItem.Click += new System.EventHandler(this.AddCommandToolStripMenuItem_Click);
            // 
            // aboveToolStripMenuItem
            // 
            this.aboveToolStripMenuItem.Enabled = false;
            this.aboveToolStripMenuItem.Name = "aboveToolStripMenuItem";
            this.aboveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.aboveToolStripMenuItem.Text = "Insert &Above";
            this.aboveToolStripMenuItem.Click += new System.EventHandler(this.AboveToolStripMenuItem_Click);
            // 
            // belowToolStripMenuItem
            // 
            this.belowToolStripMenuItem.Enabled = false;
            this.belowToolStripMenuItem.Name = "belowToolStripMenuItem";
            this.belowToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.belowToolStripMenuItem.Text = "Insert &Below";
            this.belowToolStripMenuItem.Click += new System.EventHandler(this.BelowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(160, 6);
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Enabled = false;
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.moveToolStripMenuItem.Text = "Move";
            // 
            // upToolStripMenuItem
            // 
            this.upToolStripMenuItem.Enabled = false;
            this.upToolStripMenuItem.Name = "upToolStripMenuItem";
            this.upToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.upToolStripMenuItem.Text = "&Up";
            this.upToolStripMenuItem.Click += new System.EventHandler(this.UpToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Enabled = false;
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.downToolStripMenuItem.Text = "&Down";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.DownToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.validateToolStripMenuItem});
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.textToolStripMenuItem.Text = "&Text";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Enabled = false;
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.setupToolStripMenuItem.Text = "&Setup";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.SetupToolStripMenuItem_Click);
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.Enabled = false;
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            this.validateToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.validateToolStripMenuItem.Text = "&Validate";
            this.validateToolStripMenuItem.Click += new System.EventHandler(this.ValidateToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Enabled = false;
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.testToolStripMenuItem.Text = "T&est";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.TestToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvBotData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbEnterText);
            this.splitContainer1.Panel2.Controls.Add(this.gbLoops);
            this.splitContainer1.Panel2.Controls.Add(this.gbList);
            this.splitContainer1.Panel2.Controls.Add(this.gbImageArea);
            this.splitContainer1.Panel2.Controls.Add(this.gbActionOverride);
            this.splitContainer1.Panel2.Controls.Add(this.gbAppName);
            this.splitContainer1.Panel2.Controls.Add(this.gbPickAction);
            this.splitContainer1.Panel2.Controls.Add(this.gbAppControl);
            this.splitContainer1.Panel2.Controls.Add(this.gbWFNC);
            this.splitContainer1.Panel2.Controls.Add(this.gbFindText);
            this.splitContainer1.Panel2.Controls.Add(this.gbAction);
            this.splitContainer1.Panel2.Controls.Add(this.btnUpdate);
            this.splitContainer1.Panel2.Controls.Add(this.gbSleep);
            this.splitContainer1.Panel2.Controls.Add(this.gbImageNames);
            this.splitContainer1.Panel2.Controls.Add(this.gbImageNameAndWait);
            this.splitContainer1.Panel2.Controls.Add(this.gbImageName);
            this.splitContainer1.Panel2.Controls.Add(this.gbLoopCoordinate);
            this.splitContainer1.Panel2.Controls.Add(this.gbDrag);
            this.splitContainer1.Panel2.Controls.Add(this.gbClick);
            this.splitContainer1.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer1.Size = new System.Drawing.Size(1689, 631);
            this.splitContainer1.SplitterDistance = 142;
            this.splitContainer1.TabIndex = 1;
            // 
            // tvBotData
            // 
            this.tvBotData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvBotData.Location = new System.Drawing.Point(0, 0);
            this.tvBotData.Name = "tvBotData";
            this.tvBotData.Size = new System.Drawing.Size(142, 631);
            this.tvBotData.TabIndex = 0;
            this.tvBotData.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TvBotData_BeforeSelect);
            this.tvBotData.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvBotData_AfterSelect);
            // 
            // gbEnterText
            // 
            this.gbEnterText.Controls.Add(this.tbEnterTextText);
            this.gbEnterText.Controls.Add(this.tbEnterTextOverideId);
            this.gbEnterText.Controls.Add(this.label42);
            this.gbEnterText.Controls.Add(this.label43);
            this.gbEnterText.Enabled = false;
            this.gbEnterText.Location = new System.Drawing.Point(681, 424);
            this.gbEnterText.Name = "gbEnterText";
            this.gbEnterText.Size = new System.Drawing.Size(254, 80);
            this.gbEnterText.TabIndex = 103;
            this.gbEnterText.TabStop = false;
            this.gbEnterText.Text = "Enter Text";
            this.gbEnterText.Visible = false;
            // 
            // tbEnterTextText
            // 
            this.tbEnterTextText.Location = new System.Drawing.Point(80, 22);
            this.tbEnterTextText.Name = "tbEnterTextText";
            this.tbEnterTextText.Size = new System.Drawing.Size(154, 23);
            this.tbEnterTextText.TabIndex = 1;
            this.scriptEditorToolTip.SetToolTip(this.tbEnterTextText, "Enter the unique identifier within this action\r\nthat will will be used to allow t" +
        "he number of\r\nloops to be executed, to be overridden with\r\nthe Device Config fil" +
        "e.");
            this.tbEnterTextText.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // tbEnterTextOverideId
            // 
            this.tbEnterTextOverideId.Location = new System.Drawing.Point(80, 51);
            this.tbEnterTextOverideId.Name = "tbEnterTextOverideId";
            this.tbEnterTextOverideId.Size = new System.Drawing.Size(154, 23);
            this.tbEnterTextOverideId.TabIndex = 2;
            this.scriptEditorToolTip.SetToolTip(this.tbEnterTextOverideId, "Enter the unique identifier within this action\r\nthat will will be used to allow t" +
        "he number of\r\nloops to be executed, to be overridden with\r\nthe Device Config fil" +
        "e.");
            this.tbEnterTextOverideId.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(9, 54);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(65, 15);
            this.label42.TabIndex = 12;
            this.label42.Text = "Override Id";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(9, 25);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(28, 15);
            this.label43.TabIndex = 10;
            this.label43.Text = "Text";
            // 
            // gbLoops
            // 
            this.gbLoops.Controls.Add(this.tbLoopsOverrideId);
            this.gbLoops.Controls.Add(this.label39);
            this.gbLoops.Controls.Add(this.label38);
            this.gbLoops.Controls.Add(this.tbLoopsCounter);
            this.gbLoops.Enabled = false;
            this.gbLoops.Location = new System.Drawing.Point(576, 323);
            this.gbLoops.Name = "gbLoops";
            this.gbLoops.Size = new System.Drawing.Size(254, 80);
            this.gbLoops.TabIndex = 102;
            this.gbLoops.TabStop = false;
            this.gbLoops.Text = "Loops";
            this.gbLoops.Visible = false;
            // 
            // tbLoopsOverrideId
            // 
            this.tbLoopsOverrideId.Location = new System.Drawing.Point(80, 51);
            this.tbLoopsOverrideId.Name = "tbLoopsOverrideId";
            this.tbLoopsOverrideId.Size = new System.Drawing.Size(154, 23);
            this.tbLoopsOverrideId.TabIndex = 13;
            this.scriptEditorToolTip.SetToolTip(this.tbLoopsOverrideId, "Enter the unique identifier within this action\r\nthat will will be used to allow t" +
        "he number of\r\nloops to be executed, to be overridden with\r\nthe Device Config fil" +
        "e.");
            this.tbLoopsOverrideId.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(9, 54);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(65, 15);
            this.label39.TabIndex = 12;
            this.label39.Text = "Override Id";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(9, 25);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(65, 15);
            this.label38.TabIndex = 10;
            this.label38.Text = "Number of";
            // 
            // tbLoopsCounter
            // 
            this.tbLoopsCounter.Location = new System.Drawing.Point(80, 22);
            this.tbLoopsCounter.Mask = "#00000";
            this.tbLoopsCounter.Name = "tbLoopsCounter";
            this.tbLoopsCounter.Size = new System.Drawing.Size(71, 23);
            this.tbLoopsCounter.TabIndex = 11;
            this.tbLoopsCounter.ValidatingType = typeof(int);
            this.tbLoopsCounter.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbLoopsCounter.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // gbList
            // 
            this.gbList.Controls.Add(this.tbListName);
            this.gbList.Controls.Add(this.label34);
            this.gbList.Enabled = false;
            this.gbList.Location = new System.Drawing.Point(974, 78);
            this.gbList.Name = "gbList";
            this.gbList.Size = new System.Drawing.Size(255, 48);
            this.gbList.TabIndex = 101;
            this.gbList.TabStop = false;
            this.gbList.Text = "List";
            this.gbList.Visible = false;
            // 
            // tbListName
            // 
            this.tbListName.Location = new System.Drawing.Point(73, 19);
            this.tbListName.Name = "tbListName";
            this.tbListName.Size = new System.Drawing.Size(169, 23);
            this.tbListName.TabIndex = 1;
            this.tbListName.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(7, 23);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(60, 15);
            this.label34.TabIndex = 0;
            this.label34.Text = "List Name";
            // 
            // gbImageArea
            // 
            this.gbImageArea.Controls.Add(this.btImageAreaRemove);
            this.gbImageArea.Controls.Add(this.btImageAreaAdd);
            this.gbImageArea.Controls.Add(this.tbImageAreasX);
            this.gbImageArea.Controls.Add(this.label32);
            this.gbImageArea.Controls.Add(this.tbImageAreasH);
            this.gbImageArea.Controls.Add(this.tbImageAreasY);
            this.gbImageArea.Controls.Add(this.label33);
            this.gbImageArea.Controls.Add(this.tbImageAreasW);
            this.gbImageArea.Controls.Add(this.label31);
            this.gbImageArea.Controls.Add(this.lbImageAreaAreas);
            this.gbImageArea.Controls.Add(this.label30);
            this.gbImageArea.Controls.Add(this.cbImageAreasImage);
            this.gbImageArea.Enabled = false;
            this.gbImageArea.Location = new System.Drawing.Point(834, 184);
            this.gbImageArea.Name = "gbImageArea";
            this.gbImageArea.Size = new System.Drawing.Size(305, 219);
            this.gbImageArea.TabIndex = 100;
            this.gbImageArea.TabStop = false;
            this.gbImageArea.Text = "Image Areas";
            this.gbImageArea.Visible = false;
            // 
            // btImageAreaRemove
            // 
            this.btImageAreaRemove.Location = new System.Drawing.Point(224, 186);
            this.btImageAreaRemove.Name = "btImageAreaRemove";
            this.btImageAreaRemove.Size = new System.Drawing.Size(75, 23);
            this.btImageAreaRemove.TabIndex = 17;
            this.btImageAreaRemove.Text = "Remove";
            this.btImageAreaRemove.UseVisualStyleBackColor = true;
            this.btImageAreaRemove.Click += new System.EventHandler(this.BtImageAreaRemove_Click);
            // 
            // btImageAreaAdd
            // 
            this.btImageAreaAdd.Location = new System.Drawing.Point(75, 186);
            this.btImageAreaAdd.Name = "btImageAreaAdd";
            this.btImageAreaAdd.Size = new System.Drawing.Size(75, 23);
            this.btImageAreaAdd.TabIndex = 16;
            this.btImageAreaAdd.Text = "Add";
            this.btImageAreaAdd.UseVisualStyleBackColor = true;
            this.btImageAreaAdd.Click += new System.EventHandler(this.BtImageAreaAdd_Click);
            // 
            // tbImageAreasX
            // 
            this.tbImageAreasX.Location = new System.Drawing.Point(75, 157);
            this.tbImageAreasX.Mask = "000";
            this.tbImageAreasX.Name = "tbImageAreasX";
            this.tbImageAreasX.Size = new System.Drawing.Size(26, 23);
            this.tbImageAreasX.TabIndex = 11;
            this.tbImageAreasX.ValidatingType = typeof(int);
            this.tbImageAreasX.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 160);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(42, 15);
            this.label32.TabIndex = 10;
            this.label32.Text = "Search";
            // 
            // tbImageAreasH
            // 
            this.tbImageAreasH.Location = new System.Drawing.Point(201, 157);
            this.tbImageAreasH.Mask = "000";
            this.tbImageAreasH.Name = "tbImageAreasH";
            this.tbImageAreasH.Size = new System.Drawing.Size(26, 23);
            this.tbImageAreasH.TabIndex = 15;
            this.tbImageAreasH.ValidatingType = typeof(int);
            this.tbImageAreasH.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbImageAreasY
            // 
            this.tbImageAreasY.Location = new System.Drawing.Point(107, 157);
            this.tbImageAreasY.Mask = "000";
            this.tbImageAreasY.Name = "tbImageAreasY";
            this.tbImageAreasY.Size = new System.Drawing.Size(26, 23);
            this.tbImageAreasY.TabIndex = 12;
            this.tbImageAreasY.ValidatingType = typeof(int);
            this.tbImageAreasY.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(139, 161);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(19, 15);
            this.label33.TabIndex = 13;
            this.label33.Text = "To";
            // 
            // tbImageAreasW
            // 
            this.tbImageAreasW.Location = new System.Drawing.Point(169, 157);
            this.tbImageAreasW.Mask = "000";
            this.tbImageAreasW.Name = "tbImageAreasW";
            this.tbImageAreasW.Size = new System.Drawing.Size(26, 23);
            this.tbImageAreasW.TabIndex = 14;
            this.tbImageAreasW.ValidatingType = typeof(int);
            this.tbImageAreasW.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 55);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(36, 15);
            this.label31.TabIndex = 3;
            this.label31.Text = "Areas";
            // 
            // lbImageAreaAreas
            // 
            this.lbImageAreaAreas.AllowDrop = true;
            this.lbImageAreaAreas.FormattingEnabled = true;
            this.lbImageAreaAreas.ItemHeight = 15;
            this.lbImageAreaAreas.Location = new System.Drawing.Point(75, 55);
            this.lbImageAreaAreas.Name = "lbImageAreaAreas";
            this.lbImageAreaAreas.Size = new System.Drawing.Size(224, 94);
            this.lbImageAreaAreas.TabIndex = 2;
            this.lbImageAreaAreas.SelectedIndexChanged += new System.EventHandler(this.LbImageAreaAreas_SelectedIndexChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(6, 26);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(40, 15);
            this.label30.TabIndex = 0;
            this.label30.Text = "Image";
            // 
            // cbImageAreasImage
            // 
            this.cbImageAreasImage.FormattingEnabled = true;
            this.cbImageAreasImage.Location = new System.Drawing.Point(75, 23);
            this.cbImageAreasImage.Name = "cbImageAreasImage";
            this.cbImageAreasImage.Size = new System.Drawing.Size(224, 23);
            this.cbImageAreasImage.Sorted = true;
            this.cbImageAreasImage.TabIndex = 1;
            this.cbImageAreasImage.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // gbActionOverride
            // 
            this.gbActionOverride.Controls.Add(this.btActionOverridesEdit);
            this.gbActionOverride.Controls.Add(this.lvActionOverridesOverride);
            this.gbActionOverride.Controls.Add(this.tbActionOverrideValue);
            this.gbActionOverride.Controls.Add(this.label41);
            this.gbActionOverride.Controls.Add(this.btActionOverridesRemove);
            this.gbActionOverride.Controls.Add(this.btActionOverridesAdd);
            this.gbActionOverride.Controls.Add(this.tbActionOverrideOverrideId);
            this.gbActionOverride.Controls.Add(this.label40);
            this.gbActionOverride.Controls.Add(this.label29);
            this.gbActionOverride.Controls.Add(this.dtpActionOverrideLastRun);
            this.gbActionOverride.Controls.Add(this.tbActionOverrideEnabled);
            this.gbActionOverride.Controls.Add(this.dtptbActionOverrideTimeOfDay);
            this.gbActionOverride.Controls.Add(this.label26);
            this.gbActionOverride.Controls.Add(this.tbActionOverrideName);
            this.gbActionOverride.Controls.Add(this.label27);
            this.gbActionOverride.Controls.Add(this.tbActionOverrideFrequency);
            this.gbActionOverride.Controls.Add(this.label28);
            this.gbActionOverride.Enabled = false;
            this.gbActionOverride.Location = new System.Drawing.Point(1158, 143);
            this.gbActionOverride.Name = "gbActionOverride";
            this.gbActionOverride.Size = new System.Drawing.Size(364, 392);
            this.gbActionOverride.TabIndex = 24;
            this.gbActionOverride.TabStop = false;
            this.gbActionOverride.Text = "Action Overrides";
            this.gbActionOverride.Visible = false;
            // 
            // btActionOverridesEdit
            // 
            this.btActionOverridesEdit.Location = new System.Drawing.Point(90, 362);
            this.btActionOverridesEdit.Name = "btActionOverridesEdit";
            this.btActionOverridesEdit.Size = new System.Drawing.Size(75, 23);
            this.btActionOverridesEdit.TabIndex = 23;
            this.btActionOverridesEdit.Text = "Edit";
            this.btActionOverridesEdit.UseVisualStyleBackColor = true;
            this.btActionOverridesEdit.Click += new System.EventHandler(this.btActionOverridesEdit_Click);
            // 
            // lvActionOverridesOverride
            // 
            this.lvActionOverridesOverride.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1});
            this.lvActionOverridesOverride.HideSelection = false;
            this.lvActionOverridesOverride.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lvActionOverridesOverride.Location = new System.Drawing.Point(94, 156);
            this.lvActionOverridesOverride.MultiSelect = false;
            this.lvActionOverridesOverride.Name = "lvActionOverridesOverride";
            this.lvActionOverridesOverride.ShowGroups = false;
            this.lvActionOverridesOverride.Size = new System.Drawing.Size(264, 140);
            this.lvActionOverridesOverride.TabIndex = 22;
            this.lvActionOverridesOverride.UseCompatibleStateImageBehavior = false;
            this.lvActionOverridesOverride.View = System.Windows.Forms.View.Details;
            this.lvActionOverridesOverride.SelectedIndexChanged += new System.EventHandler(this.lvActionOverridesOverride_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Override Id";
            this.columnHeader3.Width = 180;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Value";
            // 
            // tbActionOverrideValue
            // 
            this.tbActionOverrideValue.Location = new System.Drawing.Point(94, 334);
            this.tbActionOverrideValue.Name = "tbActionOverrideValue";
            this.tbActionOverrideValue.Size = new System.Drawing.Size(154, 23);
            this.tbActionOverrideValue.TabIndex = 21;
            this.scriptEditorToolTip.SetToolTip(this.tbActionOverrideValue, "Enter the unique identifier within this action\r\nthat will will be used to allow t" +
        "he number of\r\nloops to be executed, to be overridden with\r\nthe Device Config fil" +
        "e.");
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(7, 337);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(35, 15);
            this.label41.TabIndex = 20;
            this.label41.Text = "Value";
            // 
            // btActionOverridesRemove
            // 
            this.btActionOverridesRemove.Location = new System.Drawing.Point(170, 362);
            this.btActionOverridesRemove.Name = "btActionOverridesRemove";
            this.btActionOverridesRemove.Size = new System.Drawing.Size(75, 23);
            this.btActionOverridesRemove.TabIndex = 19;
            this.btActionOverridesRemove.Text = "Remove";
            this.btActionOverridesRemove.UseVisualStyleBackColor = true;
            this.btActionOverridesRemove.Click += new System.EventHandler(this.btActionOverridesRemove_Click);
            // 
            // btActionOverridesAdd
            // 
            this.btActionOverridesAdd.Location = new System.Drawing.Point(8, 362);
            this.btActionOverridesAdd.Name = "btActionOverridesAdd";
            this.btActionOverridesAdd.Size = new System.Drawing.Size(75, 23);
            this.btActionOverridesAdd.TabIndex = 18;
            this.btActionOverridesAdd.Text = "Add";
            this.btActionOverridesAdd.UseVisualStyleBackColor = true;
            this.btActionOverridesAdd.Click += new System.EventHandler(this.btActionOverridesAdd_Click);
            // 
            // tbActionOverrideOverrideId
            // 
            this.tbActionOverrideOverrideId.Location = new System.Drawing.Point(94, 303);
            this.tbActionOverrideOverrideId.Name = "tbActionOverrideOverrideId";
            this.tbActionOverrideOverrideId.Size = new System.Drawing.Size(154, 23);
            this.tbActionOverrideOverrideId.TabIndex = 15;
            this.scriptEditorToolTip.SetToolTip(this.tbActionOverrideOverrideId, "Enter the unique identifier within this action\r\nthat will will be used to allow t" +
        "he number of\r\nloops to be executed, to be overridden with\r\nthe Device Config fil" +
        "e.");
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(7, 306);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(65, 15);
            this.label40.TabIndex = 14;
            this.label40.Text = "Override Id";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(6, 51);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(52, 15);
            this.label29.TabIndex = 2;
            this.label29.Text = "Last Run";
            // 
            // dtpActionOverrideLastRun
            // 
            this.dtpActionOverrideLastRun.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtpActionOverrideLastRun.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpActionOverrideLastRun.Location = new System.Drawing.Point(94, 47);
            this.dtpActionOverrideLastRun.Name = "dtpActionOverrideLastRun";
            this.dtpActionOverrideLastRun.Size = new System.Drawing.Size(154, 23);
            this.dtpActionOverrideLastRun.TabIndex = 3;
            this.dtpActionOverrideLastRun.ValueChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // tbActionOverrideEnabled
            // 
            this.tbActionOverrideEnabled.AutoSize = true;
            this.tbActionOverrideEnabled.Location = new System.Drawing.Point(6, 76);
            this.tbActionOverrideEnabled.Name = "tbActionOverrideEnabled";
            this.tbActionOverrideEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tbActionOverrideEnabled.Size = new System.Drawing.Size(85, 19);
            this.tbActionOverrideEnabled.TabIndex = 4;
            this.tbActionOverrideEnabled.Text = "    ?Enabled";
            this.tbActionOverrideEnabled.UseVisualStyleBackColor = true;
            this.tbActionOverrideEnabled.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // dtptbActionOverrideTimeOfDay
            // 
            this.dtptbActionOverrideTimeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtptbActionOverrideTimeOfDay.Location = new System.Drawing.Point(94, 126);
            this.dtptbActionOverrideTimeOfDay.Name = "dtptbActionOverrideTimeOfDay";
            this.dtptbActionOverrideTimeOfDay.ShowCheckBox = true;
            this.dtptbActionOverrideTimeOfDay.ShowUpDown = true;
            this.dtptbActionOverrideTimeOfDay.Size = new System.Drawing.Size(109, 23);
            this.dtptbActionOverrideTimeOfDay.TabIndex = 8;
            this.dtptbActionOverrideTimeOfDay.Value = new System.DateTime(2022, 4, 12, 10, 0, 0, 0);
            this.dtptbActionOverrideTimeOfDay.ValueChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 132);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(66, 15);
            this.label26.TabIndex = 7;
            this.label26.Text = "TimeOfDay";
            // 
            // tbActionOverrideName
            // 
            this.tbActionOverrideName.Location = new System.Drawing.Point(94, 18);
            this.tbActionOverrideName.Name = "tbActionOverrideName";
            this.tbActionOverrideName.ReadOnly = true;
            this.tbActionOverrideName.Size = new System.Drawing.Size(154, 23);
            this.tbActionOverrideName.TabIndex = 1;
            this.tbActionOverrideName.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(6, 21);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(39, 15);
            this.label27.TabIndex = 0;
            this.label27.Text = "Name";
            // 
            // tbActionOverrideFrequency
            // 
            this.tbActionOverrideFrequency.Location = new System.Drawing.Point(94, 98);
            this.tbActionOverrideFrequency.Mask = "#0000";
            this.tbActionOverrideFrequency.Name = "tbActionOverrideFrequency";
            this.tbActionOverrideFrequency.Size = new System.Drawing.Size(71, 23);
            this.tbActionOverrideFrequency.TabIndex = 6;
            this.tbActionOverrideFrequency.ValidatingType = typeof(int);
            this.tbActionOverrideFrequency.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbActionOverrideFrequency.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 106);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(84, 15);
            this.label28.TabIndex = 5;
            this.label28.Text = "Frequency (m)";
            // 
            // gbAppName
            // 
            this.gbAppName.Controls.Add(this.label25);
            this.gbAppName.Controls.Add(this.tbAppNameTimeout);
            this.gbAppName.Controls.Add(this.label24);
            this.gbAppName.Controls.Add(this.tbAppNameAppId);
            this.gbAppName.Enabled = false;
            this.gbAppName.Location = new System.Drawing.Point(329, 518);
            this.gbAppName.Name = "gbAppName";
            this.gbAppName.Size = new System.Drawing.Size(438, 84);
            this.gbAppName.TabIndex = 23;
            this.gbAppName.TabStop = false;
            this.gbAppName.Text = "App Name";
            this.gbAppName.Visible = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(7, 56);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(79, 15);
            this.label25.TabIndex = 2;
            this.label25.Text = "Wait Time ms";
            // 
            // tbAppNameTimeout
            // 
            this.tbAppNameTimeout.Location = new System.Drawing.Point(92, 53);
            this.tbAppNameTimeout.Mask = "#00000";
            this.tbAppNameTimeout.Name = "tbAppNameTimeout";
            this.tbAppNameTimeout.Size = new System.Drawing.Size(71, 23);
            this.tbAppNameTimeout.TabIndex = 3;
            this.tbAppNameTimeout.ValidatingType = typeof(int);
            this.tbAppNameTimeout.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbAppNameTimeout.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(7, 27);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(42, 15);
            this.label24.TabIndex = 0;
            this.label24.Text = "App Id";
            // 
            // tbAppNameAppId
            // 
            this.tbAppNameAppId.Location = new System.Drawing.Point(77, 20);
            this.tbAppNameAppId.Name = "tbAppNameAppId";
            this.tbAppNameAppId.Size = new System.Drawing.Size(355, 23);
            this.tbAppNameAppId.TabIndex = 1;
            this.tbAppNameAppId.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // gbPickAction
            // 
            this.gbPickAction.Controls.Add(this.cbPickActionAction);
            this.gbPickAction.Controls.Add(this.label23);
            this.gbPickAction.Enabled = false;
            this.gbPickAction.Location = new System.Drawing.Point(773, 536);
            this.gbPickAction.Name = "gbPickAction";
            this.gbPickAction.Size = new System.Drawing.Size(293, 58);
            this.gbPickAction.TabIndex = 22;
            this.gbPickAction.TabStop = false;
            this.gbPickAction.Text = "Action";
            this.gbPickAction.Visible = false;
            // 
            // cbPickActionAction
            // 
            this.cbPickActionAction.FormattingEnabled = true;
            this.cbPickActionAction.Location = new System.Drawing.Point(55, 21);
            this.cbPickActionAction.Name = "cbPickActionAction";
            this.cbPickActionAction.Size = new System.Drawing.Size(232, 23);
            this.cbPickActionAction.Sorted = true;
            this.cbPickActionAction.TabIndex = 1;
            this.cbPickActionAction.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 24);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(42, 15);
            this.label23.TabIndex = 0;
            this.label23.Text = "Action";
            // 
            // gbAppControl
            // 
            this.gbAppControl.Controls.Add(this.label20);
            this.gbAppControl.Controls.Add(this.tbAppControlWait);
            this.gbAppControl.Controls.Add(this.tbAppControlName);
            this.gbAppControl.Controls.Add(this.label19);
            this.gbAppControl.Enabled = false;
            this.gbAppControl.Location = new System.Drawing.Point(329, 412);
            this.gbAppControl.Name = "gbAppControl";
            this.gbAppControl.Size = new System.Drawing.Size(324, 100);
            this.gbAppControl.TabIndex = 21;
            this.gbAppControl.TabStop = false;
            this.gbAppControl.Text = "App Control";
            this.gbAppControl.Visible = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 56);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(79, 15);
            this.label20.TabIndex = 2;
            this.label20.Text = "Wait Time ms";
            // 
            // tbAppControlWait
            // 
            this.tbAppControlWait.Location = new System.Drawing.Point(93, 53);
            this.tbAppControlWait.Mask = "#00000";
            this.tbAppControlWait.Name = "tbAppControlWait";
            this.tbAppControlWait.Size = new System.Drawing.Size(71, 23);
            this.tbAppControlWait.TabIndex = 3;
            this.tbAppControlWait.ValidatingType = typeof(int);
            this.tbAppControlWait.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbAppControlWait.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbAppControlName
            // 
            this.tbAppControlName.Location = new System.Drawing.Point(52, 21);
            this.tbAppControlName.Name = "tbAppControlName";
            this.tbAppControlName.Size = new System.Drawing.Size(154, 23);
            this.tbAppControlName.TabIndex = 1;
            this.tbAppControlName.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 25);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(39, 15);
            this.label19.TabIndex = 0;
            this.label19.Text = "Name";
            // 
            // gbWFNC
            // 
            this.gbWFNC.Controls.Add(this.nudWFNCDetectPercent);
            this.gbWFNC.Controls.Add(this.label18);
            this.gbWFNC.Controls.Add(this.label17);
            this.gbWFNC.Controls.Add(this.tbWFNCWait);
            this.gbWFNC.Controls.Add(this.tbWFNCX1);
            this.gbWFNC.Controls.Add(this.label15);
            this.gbWFNC.Controls.Add(this.tbWFNCY2);
            this.gbWFNC.Controls.Add(this.tbWFNCY1);
            this.gbWFNC.Controls.Add(this.label16);
            this.gbWFNC.Controls.Add(this.tbWFNCX2);
            this.gbWFNC.Enabled = false;
            this.gbWFNC.Location = new System.Drawing.Point(460, 184);
            this.gbWFNC.Name = "gbWFNC";
            this.gbWFNC.Size = new System.Drawing.Size(330, 103);
            this.gbWFNC.TabIndex = 19;
            this.gbWFNC.TabStop = false;
            this.gbWFNC.Text = "Wait For (No) Change";
            this.gbWFNC.Visible = false;
            // 
            // nudWFNCDetectPercent
            // 
            this.nudWFNCDetectPercent.Location = new System.Drawing.Point(119, 47);
            this.nudWFNCDetectPercent.Name = "nudWFNCDetectPercent";
            this.nudWFNCDetectPercent.Size = new System.Drawing.Size(47, 23);
            this.nudWFNCDetectPercent.TabIndex = 7;
            this.nudWFNCDetectPercent.ValueChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(103, 15);
            this.label18.TabIndex = 6;
            this.label18.Text = "Detect Percentage";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 76);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 15);
            this.label17.TabIndex = 8;
            this.label17.Text = "Wait Time ms";
            // 
            // tbWFNCWait
            // 
            this.tbWFNCWait.Location = new System.Drawing.Point(95, 73);
            this.tbWFNCWait.Mask = "#00000";
            this.tbWFNCWait.Name = "tbWFNCWait";
            this.tbWFNCWait.Size = new System.Drawing.Size(71, 23);
            this.tbWFNCWait.TabIndex = 9;
            this.tbWFNCWait.ValidatingType = typeof(int);
            this.tbWFNCWait.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbWFNCWait.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbWFNCX1
            // 
            this.tbWFNCX1.Location = new System.Drawing.Point(54, 20);
            this.tbWFNCX1.Mask = "000";
            this.tbWFNCX1.Name = "tbWFNCX1";
            this.tbWFNCX1.Size = new System.Drawing.Size(26, 23);
            this.tbWFNCX1.TabIndex = 1;
            this.tbWFNCX1.ValidatingType = typeof(int);
            this.tbWFNCX1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbWFNCX1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(42, 15);
            this.label15.TabIndex = 0;
            this.label15.Text = "Search";
            // 
            // tbWFNCY2
            // 
            this.tbWFNCY2.Location = new System.Drawing.Point(180, 20);
            this.tbWFNCY2.Mask = "000";
            this.tbWFNCY2.Name = "tbWFNCY2";
            this.tbWFNCY2.Size = new System.Drawing.Size(26, 23);
            this.tbWFNCY2.TabIndex = 5;
            this.tbWFNCY2.ValidatingType = typeof(int);
            this.tbWFNCY2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbWFNCY2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbWFNCY1
            // 
            this.tbWFNCY1.Location = new System.Drawing.Point(86, 20);
            this.tbWFNCY1.Mask = "000";
            this.tbWFNCY1.Name = "tbWFNCY1";
            this.tbWFNCY1.Size = new System.Drawing.Size(26, 23);
            this.tbWFNCY1.TabIndex = 2;
            this.tbWFNCY1.ValidatingType = typeof(int);
            this.tbWFNCY1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbWFNCY1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(118, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(19, 15);
            this.label16.TabIndex = 3;
            this.label16.Text = "To";
            // 
            // tbWFNCX2
            // 
            this.tbWFNCX2.Location = new System.Drawing.Point(148, 20);
            this.tbWFNCX2.Mask = "000";
            this.tbWFNCX2.Name = "tbWFNCX2";
            this.tbWFNCX2.Size = new System.Drawing.Size(26, 23);
            this.tbWFNCX2.TabIndex = 4;
            this.tbWFNCX2.ValidatingType = typeof(int);
            this.tbWFNCX2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbWFNCX2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // gbFindText
            // 
            this.gbFindText.Controls.Add(this.btnFindTextGenerate);
            this.gbFindText.Controls.Add(this.btnPastFindText);
            this.gbFindText.Controls.Add(this.label13);
            this.gbFindText.Controls.Add(this.tbFindTextBackTolerance);
            this.gbFindText.Controls.Add(this.tbFindTextTextTolerance);
            this.gbFindText.Controls.Add(this.label12);
            this.gbFindText.Controls.Add(this.tbFindTextSearchX1);
            this.gbFindText.Controls.Add(this.label10);
            this.gbFindText.Controls.Add(this.tbFindTextSearchY2);
            this.gbFindText.Controls.Add(this.tbFindTextSearchY1);
            this.gbFindText.Controls.Add(this.label11);
            this.gbFindText.Controls.Add(this.tbFindTextSearchX2);
            this.gbFindText.Controls.Add(this.tbFindTextSearch);
            this.gbFindText.Controls.Add(this.label9);
            this.gbFindText.Controls.Add(this.tbFindTextName);
            this.gbFindText.Controls.Add(this.label8);
            this.gbFindText.Enabled = false;
            this.gbFindText.Location = new System.Drawing.Point(460, 10);
            this.gbFindText.Name = "gbFindText";
            this.gbFindText.Size = new System.Drawing.Size(507, 168);
            this.gbFindText.TabIndex = 18;
            this.gbFindText.TabStop = false;
            this.gbFindText.Text = "FindText";
            this.gbFindText.Visible = false;
            // 
            // btnFindTextGenerate
            // 
            this.btnFindTextGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindTextGenerate.Location = new System.Drawing.Point(344, 137);
            this.btnFindTextGenerate.Name = "btnFindTextGenerate";
            this.btnFindTextGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnFindTextGenerate.TabIndex = 15;
            this.btnFindTextGenerate.Text = "Generate";
            this.btnFindTextGenerate.UseVisualStyleBackColor = true;
            this.btnFindTextGenerate.Click += new System.EventHandler(this.BtnFindTextGenerate_Click);
            // 
            // btnPastFindText
            // 
            this.btnPastFindText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPastFindText.Location = new System.Drawing.Point(426, 137);
            this.btnPastFindText.Name = "btnPastFindText";
            this.btnPastFindText.Size = new System.Drawing.Size(75, 23);
            this.btnPastFindText.TabIndex = 14;
            this.btnPastFindText.Text = "Paste";
            this.btnPastFindText.UseVisualStyleBackColor = true;
            this.btnPastFindText.Click += new System.EventHandler(this.BtnPasteFindText_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 138);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(85, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "Back Tolerance";
            // 
            // tbFindTextBackTolerance
            // 
            this.tbFindTextBackTolerance.Location = new System.Drawing.Point(95, 131);
            this.tbFindTextBackTolerance.Mask = "0.000";
            this.tbFindTextBackTolerance.Name = "tbFindTextBackTolerance";
            this.tbFindTextBackTolerance.Size = new System.Drawing.Size(40, 23);
            this.tbFindTextBackTolerance.TabIndex = 13;
            this.tbFindTextBackTolerance.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextBackTolerance.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbFindTextTextTolerance
            // 
            this.tbFindTextTextTolerance.Location = new System.Drawing.Point(95, 102);
            this.tbFindTextTextTolerance.Mask = "0.000";
            this.tbFindTextTextTolerance.Name = "tbFindTextTextTolerance";
            this.tbFindTextTextTolerance.Size = new System.Drawing.Size(40, 23);
            this.tbFindTextTextTolerance.TabIndex = 11;
            this.tbFindTextTextTolerance.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextTextTolerance.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 107);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "Text Tolerance";
            // 
            // tbFindTextSearchX1
            // 
            this.tbFindTextSearchX1.Location = new System.Drawing.Point(52, 73);
            this.tbFindTextSearchX1.Mask = "000";
            this.tbFindTextSearchX1.Name = "tbFindTextSearchX1";
            this.tbFindTextSearchX1.Size = new System.Drawing.Size(26, 23);
            this.tbFindTextSearchX1.TabIndex = 5;
            this.tbFindTextSearchX1.ValidatingType = typeof(int);
            this.tbFindTextSearchX1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextSearchX1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "Search";
            // 
            // tbFindTextSearchY2
            // 
            this.tbFindTextSearchY2.Location = new System.Drawing.Point(178, 73);
            this.tbFindTextSearchY2.Mask = "000";
            this.tbFindTextSearchY2.Name = "tbFindTextSearchY2";
            this.tbFindTextSearchY2.Size = new System.Drawing.Size(26, 23);
            this.tbFindTextSearchY2.TabIndex = 9;
            this.tbFindTextSearchY2.ValidatingType = typeof(int);
            this.tbFindTextSearchY2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextSearchY2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbFindTextSearchY1
            // 
            this.tbFindTextSearchY1.Location = new System.Drawing.Point(84, 73);
            this.tbFindTextSearchY1.Mask = "000";
            this.tbFindTextSearchY1.Name = "tbFindTextSearchY1";
            this.tbFindTextSearchY1.Size = new System.Drawing.Size(26, 23);
            this.tbFindTextSearchY1.TabIndex = 6;
            this.tbFindTextSearchY1.ValidatingType = typeof(int);
            this.tbFindTextSearchY1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextSearchY1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(116, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(19, 15);
            this.label11.TabIndex = 7;
            this.label11.Text = "To";
            // 
            // tbFindTextSearchX2
            // 
            this.tbFindTextSearchX2.Location = new System.Drawing.Point(146, 73);
            this.tbFindTextSearchX2.Mask = "000";
            this.tbFindTextSearchX2.Name = "tbFindTextSearchX2";
            this.tbFindTextSearchX2.Size = new System.Drawing.Size(26, 23);
            this.tbFindTextSearchX2.TabIndex = 8;
            this.tbFindTextSearchX2.ValidatingType = typeof(int);
            this.tbFindTextSearchX2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbFindTextSearchX2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbFindTextSearch
            // 
            this.tbFindTextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFindTextSearch.Location = new System.Drawing.Point(53, 44);
            this.tbFindTextSearch.Name = "tbFindTextSearch";
            this.tbFindTextSearch.Size = new System.Drawing.Size(450, 23);
            this.tbFindTextSearch.TabIndex = 3;
            this.tbFindTextSearch.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "Find";
            // 
            // tbFindTextName
            // 
            this.tbFindTextName.Location = new System.Drawing.Point(53, 14);
            this.tbFindTextName.Name = "tbFindTextName";
            this.tbFindTextName.Size = new System.Drawing.Size(154, 23);
            this.tbFindTextName.TabIndex = 1;
            this.tbFindTextName.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "Name";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.cbActionAfter);
            this.gbAction.Controls.Add(this.label37);
            this.gbAction.Controls.Add(this.cbActionBefore);
            this.gbAction.Controls.Add(this.label36);
            this.gbAction.Controls.Add(this.label22);
            this.gbAction.Controls.Add(this.cbActionType);
            this.gbAction.Controls.Add(this.dtpActionTimeOfDay);
            this.gbAction.Controls.Add(this.label21);
            this.gbAction.Controls.Add(this.tbActionName);
            this.gbAction.Controls.Add(this.label14);
            this.gbAction.Controls.Add(this.tbActionFrequency);
            this.gbAction.Controls.Add(this.label7);
            this.gbAction.Enabled = false;
            this.gbAction.Location = new System.Drawing.Point(5, 344);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(308, 206);
            this.gbAction.TabIndex = 17;
            this.gbAction.TabStop = false;
            this.gbAction.Text = "Action";
            this.gbAction.Visible = false;
            // 
            // cbActionAfter
            // 
            this.cbActionAfter.FormattingEnabled = true;
            this.cbActionAfter.Location = new System.Drawing.Point(57, 168);
            this.cbActionAfter.Name = "cbActionAfter";
            this.cbActionAfter.Size = new System.Drawing.Size(232, 23);
            this.cbActionAfter.Sorted = true;
            this.cbActionAfter.TabIndex = 11;
            this.cbActionAfter.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(9, 171);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(33, 15);
            this.label37.TabIndex = 10;
            this.label37.Text = "After";
            // 
            // cbActionBefore
            // 
            this.cbActionBefore.FormattingEnabled = true;
            this.cbActionBefore.Location = new System.Drawing.Point(57, 137);
            this.cbActionBefore.Name = "cbActionBefore";
            this.cbActionBefore.Size = new System.Drawing.Size(232, 23);
            this.cbActionBefore.Sorted = true;
            this.cbActionBefore.TabIndex = 9;
            this.cbActionBefore.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(9, 140);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(41, 15);
            this.label36.TabIndex = 8;
            this.label36.Text = "Before";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(0, 55);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(31, 15);
            this.label22.TabIndex = 2;
            this.label22.Text = "Type";
            // 
            // cbActionType
            // 
            this.cbActionType.FormattingEnabled = true;
            this.cbActionType.Items.AddRange(new object[] {
            "Scheduled",
            "Daily",
            "Adhoc",
            "Always",
            "System"});
            this.cbActionType.Location = new System.Drawing.Point(49, 52);
            this.cbActionType.Name = "cbActionType";
            this.cbActionType.Size = new System.Drawing.Size(154, 23);
            this.cbActionType.TabIndex = 3;
            this.cbActionType.TextChanged += new System.EventHandler(this.CbActionType_TextChanged);
            // 
            // dtpActionTimeOfDay
            // 
            this.dtpActionTimeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpActionTimeOfDay.Location = new System.Drawing.Point(91, 105);
            this.dtpActionTimeOfDay.Name = "dtpActionTimeOfDay";
            this.dtpActionTimeOfDay.ShowCheckBox = true;
            this.dtpActionTimeOfDay.ShowUpDown = true;
            this.dtpActionTimeOfDay.Size = new System.Drawing.Size(109, 23);
            this.dtpActionTimeOfDay.TabIndex = 7;
            this.dtpActionTimeOfDay.Value = new System.DateTime(2022, 4, 12, 10, 0, 0, 0);
            this.dtpActionTimeOfDay.ValueChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 111);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(66, 15);
            this.label21.TabIndex = 6;
            this.label21.Text = "TimeOfDay";
            // 
            // tbActionName
            // 
            this.tbActionName.Location = new System.Drawing.Point(49, 22);
            this.tbActionName.Name = "tbActionName";
            this.tbActionName.Size = new System.Drawing.Size(154, 23);
            this.tbActionName.TabIndex = 1;
            this.tbActionName.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 15);
            this.label14.TabIndex = 0;
            this.label14.Text = "Name";
            // 
            // tbActionFrequency
            // 
            this.tbActionFrequency.Location = new System.Drawing.Point(91, 77);
            this.tbActionFrequency.Mask = "#0000";
            this.tbActionFrequency.Name = "tbActionFrequency";
            this.tbActionFrequency.Size = new System.Drawing.Size(71, 23);
            this.tbActionFrequency.TabIndex = 5;
            this.tbActionFrequency.ValidatingType = typeof(int);
            this.tbActionFrequency.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbActionFrequency.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Frequency (m)";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdate.Location = new System.Drawing.Point(12, 596);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 99;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // gbSleep
            // 
            this.gbSleep.Controls.Add(this.label5);
            this.gbSleep.Controls.Add(this.tbDelay);
            this.gbSleep.Enabled = false;
            this.gbSleep.Location = new System.Drawing.Point(973, 22);
            this.gbSleep.Name = "gbSleep";
            this.gbSleep.Size = new System.Drawing.Size(200, 50);
            this.gbSleep.TabIndex = 15;
            this.gbSleep.TabStop = false;
            this.gbSleep.Text = "Sleep";
            this.gbSleep.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Sleep Time ms";
            // 
            // tbDelay
            // 
            this.tbDelay.Location = new System.Drawing.Point(91, 16);
            this.tbDelay.Mask = "000000";
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(71, 23);
            this.tbDelay.TabIndex = 1;
            this.tbDelay.ValidatingType = typeof(int);
            this.tbDelay.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDelay.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // gbImageNames
            // 
            this.gbImageNames.Controls.Add(this.cbImageNamesMissingOk);
            this.gbImageNames.Controls.Add(this.label6);
            this.gbImageNames.Controls.Add(this.tbImageNamesWait);
            this.gbImageNames.Controls.Add(this.btnRemoveImageNames);
            this.gbImageNames.Controls.Add(this.btnAddImageNames);
            this.gbImageNames.Controls.Add(this.cbImageNamesForList);
            this.gbImageNames.Controls.Add(this.lbImageNames);
            this.gbImageNames.Enabled = false;
            this.gbImageNames.Location = new System.Drawing.Point(254, 10);
            this.gbImageNames.Name = "gbImageNames";
            this.gbImageNames.Size = new System.Drawing.Size(200, 292);
            this.gbImageNames.TabIndex = 14;
            this.gbImageNames.TabStop = false;
            this.gbImageNames.Text = "Image Names";
            this.gbImageNames.Visible = false;
            // 
            // cbImageNamesMissingOk
            // 
            this.cbImageNamesMissingOk.AutoSize = true;
            this.cbImageNamesMissingOk.Location = new System.Drawing.Point(9, 249);
            this.cbImageNamesMissingOk.Name = "cbImageNamesMissingOk";
            this.cbImageNamesMissingOk.Size = new System.Drawing.Size(90, 19);
            this.cbImageNamesMissingOk.TabIndex = 6;
            this.cbImageNamesMissingOk.Text = "Missing Ok?";
            this.cbImageNamesMissingOk.UseVisualStyleBackColor = true;
            this.cbImageNamesMissingOk.Visible = false;
            this.cbImageNamesMissingOk.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "Wait Time ms";
            // 
            // tbImageNamesWait
            // 
            this.tbImageNamesWait.Location = new System.Drawing.Point(91, 220);
            this.tbImageNamesWait.Mask = "#00000";
            this.tbImageNamesWait.Name = "tbImageNamesWait";
            this.tbImageNamesWait.Size = new System.Drawing.Size(71, 23);
            this.tbImageNamesWait.TabIndex = 5;
            this.tbImageNamesWait.ValidatingType = typeof(int);
            this.tbImageNamesWait.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbImageNamesWait.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // btnRemoveImageNames
            // 
            this.btnRemoveImageNames.Location = new System.Drawing.Point(118, 194);
            this.btnRemoveImageNames.Name = "btnRemoveImageNames";
            this.btnRemoveImageNames.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveImageNames.TabIndex = 3;
            this.btnRemoveImageNames.Text = "Remove";
            this.btnRemoveImageNames.UseVisualStyleBackColor = true;
            this.btnRemoveImageNames.Click += new System.EventHandler(this.BtnRemoveImageNames_Click);
            // 
            // btnAddImageNames
            // 
            this.btnAddImageNames.Location = new System.Drawing.Point(6, 194);
            this.btnAddImageNames.Name = "btnAddImageNames";
            this.btnAddImageNames.Size = new System.Drawing.Size(75, 23);
            this.btnAddImageNames.TabIndex = 2;
            this.btnAddImageNames.Text = "Add";
            this.btnAddImageNames.UseVisualStyleBackColor = true;
            this.btnAddImageNames.Click += new System.EventHandler(this.BtnAddImageNames_Click);
            // 
            // cbImageNamesForList
            // 
            this.cbImageNamesForList.FormattingEnabled = true;
            this.cbImageNamesForList.Location = new System.Drawing.Point(3, 164);
            this.cbImageNamesForList.Name = "cbImageNamesForList";
            this.cbImageNamesForList.Size = new System.Drawing.Size(191, 23);
            this.cbImageNamesForList.Sorted = true;
            this.cbImageNamesForList.TabIndex = 1;
            // 
            // lbImageNames
            // 
            this.lbImageNames.AllowDrop = true;
            this.lbImageNames.FormattingEnabled = true;
            this.lbImageNames.ItemHeight = 15;
            this.lbImageNames.Location = new System.Drawing.Point(3, 19);
            this.lbImageNames.Name = "lbImageNames";
            this.lbImageNames.Size = new System.Drawing.Size(194, 139);
            this.lbImageNames.TabIndex = 0;
            this.lbImageNames.SelectedValueChanged += new System.EventHandler(this.LbImageNames_SelectedValueChanged);
            // 
            // gbImageNameAndWait
            // 
            this.gbImageNameAndWait.Controls.Add(this.cbImageNameMissingOk);
            this.gbImageNameAndWait.Controls.Add(this.cbImageNameWithWait);
            this.gbImageNameAndWait.Controls.Add(this.label4);
            this.gbImageNameAndWait.Controls.Add(this.tbTimeout);
            this.gbImageNameAndWait.Enabled = false;
            this.gbImageNameAndWait.Location = new System.Drawing.Point(5, 233);
            this.gbImageNameAndWait.Name = "gbImageNameAndWait";
            this.gbImageNameAndWait.Size = new System.Drawing.Size(244, 105);
            this.gbImageNameAndWait.TabIndex = 13;
            this.gbImageNameAndWait.TabStop = false;
            this.gbImageNameAndWait.Text = "Image Name And Wait";
            this.gbImageNameAndWait.Visible = false;
            // 
            // cbImageNameMissingOk
            // 
            this.cbImageNameMissingOk.AutoSize = true;
            this.cbImageNameMissingOk.Location = new System.Drawing.Point(16, 80);
            this.cbImageNameMissingOk.Name = "cbImageNameMissingOk";
            this.cbImageNameMissingOk.Size = new System.Drawing.Size(90, 19);
            this.cbImageNameMissingOk.TabIndex = 3;
            this.cbImageNameMissingOk.Text = "Missing Ok?";
            this.cbImageNameMissingOk.UseVisualStyleBackColor = true;
            this.cbImageNameMissingOk.Visible = false;
            this.cbImageNameMissingOk.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // cbImageNameWithWait
            // 
            this.cbImageNameWithWait.FormattingEnabled = true;
            this.cbImageNameWithWait.Location = new System.Drawing.Point(8, 23);
            this.cbImageNameWithWait.Name = "cbImageNameWithWait";
            this.cbImageNameWithWait.Size = new System.Drawing.Size(230, 23);
            this.cbImageNameWithWait.Sorted = true;
            this.cbImageNameWithWait.TabIndex = 0;
            this.cbImageNameWithWait.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Wait Time ms";
            // 
            // tbTimeout
            // 
            this.tbTimeout.Location = new System.Drawing.Point(93, 51);
            this.tbTimeout.Mask = "#00000";
            this.tbTimeout.Name = "tbTimeout";
            this.tbTimeout.Size = new System.Drawing.Size(71, 23);
            this.tbTimeout.TabIndex = 2;
            this.tbTimeout.ValidatingType = typeof(int);
            this.tbTimeout.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbTimeout.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // gbImageName
            // 
            this.gbImageName.Controls.Add(this.cbIgnoreMissing);
            this.gbImageName.Controls.Add(this.cbImageNameNoWait);
            this.gbImageName.Enabled = false;
            this.gbImageName.Location = new System.Drawing.Point(329, 316);
            this.gbImageName.Name = "gbImageName";
            this.gbImageName.Size = new System.Drawing.Size(244, 90);
            this.gbImageName.TabIndex = 12;
            this.gbImageName.TabStop = false;
            this.gbImageName.Text = "Image Name";
            this.gbImageName.Visible = false;
            // 
            // cbIgnoreMissing
            // 
            this.cbIgnoreMissing.AutoSize = true;
            this.cbIgnoreMissing.Location = new System.Drawing.Point(8, 53);
            this.cbIgnoreMissing.Name = "cbIgnoreMissing";
            this.cbIgnoreMissing.Size = new System.Drawing.Size(90, 19);
            this.cbIgnoreMissing.TabIndex = 1;
            this.cbIgnoreMissing.Text = "Missing Ok?";
            this.cbIgnoreMissing.UseVisualStyleBackColor = true;
            this.cbIgnoreMissing.Visible = false;
            this.cbIgnoreMissing.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // cbImageNameNoWait
            // 
            this.cbImageNameNoWait.FormattingEnabled = true;
            this.cbImageNameNoWait.Location = new System.Drawing.Point(8, 23);
            this.cbImageNameNoWait.Name = "cbImageNameNoWait";
            this.cbImageNameNoWait.Size = new System.Drawing.Size(230, 23);
            this.cbImageNameNoWait.Sorted = true;
            this.cbImageNameNoWait.TabIndex = 0;
            this.cbImageNameNoWait.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // gbLoopCoordinate
            // 
            this.gbLoopCoordinate.Controls.Add(this.rbLoopCoordY);
            this.gbLoopCoordinate.Controls.Add(this.rbLoopCoordX);
            this.gbLoopCoordinate.Enabled = false;
            this.gbLoopCoordinate.Location = new System.Drawing.Point(4, 157);
            this.gbLoopCoordinate.Name = "gbLoopCoordinate";
            this.gbLoopCoordinate.Size = new System.Drawing.Size(245, 73);
            this.gbLoopCoordinate.TabIndex = 11;
            this.gbLoopCoordinate.TabStop = false;
            this.gbLoopCoordinate.Text = "Loop Coordinate";
            this.gbLoopCoordinate.Visible = false;
            // 
            // rbLoopCoordY
            // 
            this.rbLoopCoordY.AutoSize = true;
            this.rbLoopCoordY.Location = new System.Drawing.Point(6, 47);
            this.rbLoopCoordY.Name = "rbLoopCoordY";
            this.rbLoopCoordY.Size = new System.Drawing.Size(32, 19);
            this.rbLoopCoordY.TabIndex = 1;
            this.rbLoopCoordY.TabStop = true;
            this.rbLoopCoordY.Text = "Y";
            this.rbLoopCoordY.UseVisualStyleBackColor = true;
            this.rbLoopCoordY.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // rbLoopCoordX
            // 
            this.rbLoopCoordX.AutoSize = true;
            this.rbLoopCoordX.Location = new System.Drawing.Point(6, 22);
            this.rbLoopCoordX.Name = "rbLoopCoordX";
            this.rbLoopCoordX.Size = new System.Drawing.Size(32, 19);
            this.rbLoopCoordX.TabIndex = 0;
            this.rbLoopCoordX.TabStop = true;
            this.rbLoopCoordX.Text = "X";
            this.rbLoopCoordX.UseVisualStyleBackColor = true;
            this.rbLoopCoordX.CheckedChanged += new System.EventHandler(this.AllFields_TextChanged);
            // 
            // gbDrag
            // 
            this.gbDrag.Controls.Add(this.label35);
            this.gbDrag.Controls.Add(this.tbDragTime);
            this.gbDrag.Controls.Add(this.tbDragX1);
            this.gbDrag.Controls.Add(this.label2);
            this.gbDrag.Controls.Add(this.tbDragY2);
            this.gbDrag.Controls.Add(this.tbDragY1);
            this.gbDrag.Controls.Add(this.label3);
            this.gbDrag.Controls.Add(this.tbDragX2);
            this.gbDrag.Enabled = false;
            this.gbDrag.Location = new System.Drawing.Point(2, 65);
            this.gbDrag.Name = "gbDrag";
            this.gbDrag.Size = new System.Drawing.Size(246, 86);
            this.gbDrag.TabIndex = 10;
            this.gbDrag.TabStop = false;
            this.gbDrag.Text = "Drag";
            this.gbDrag.Visible = false;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(10, 55);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(80, 15);
            this.label35.TabIndex = 6;
            this.label35.Text = "Drag Time ms";
            // 
            // tbDragTime
            // 
            this.tbDragTime.Location = new System.Drawing.Point(95, 52);
            this.tbDragTime.Mask = "#00000";
            this.tbDragTime.Name = "tbDragTime";
            this.tbDragTime.Size = new System.Drawing.Size(71, 23);
            this.tbDragTime.TabIndex = 7;
            this.tbDragTime.ValidatingType = typeof(int);
            this.tbDragTime.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDragTime.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbDragX1
            // 
            this.tbDragX1.Location = new System.Drawing.Point(50, 22);
            this.tbDragX1.Mask = "000";
            this.tbDragX1.Name = "tbDragX1";
            this.tbDragX1.Size = new System.Drawing.Size(26, 23);
            this.tbDragX1.TabIndex = 1;
            this.tbDragX1.ValidatingType = typeof(int);
            this.tbDragX1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDragX1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "From";
            // 
            // tbDragY2
            // 
            this.tbDragY2.Location = new System.Drawing.Point(172, 22);
            this.tbDragY2.Mask = "000";
            this.tbDragY2.Name = "tbDragY2";
            this.tbDragY2.Size = new System.Drawing.Size(26, 23);
            this.tbDragY2.TabIndex = 5;
            this.tbDragY2.ValidatingType = typeof(int);
            this.tbDragY2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDragY2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbDragY1
            // 
            this.tbDragY1.Location = new System.Drawing.Point(83, 22);
            this.tbDragY1.Mask = "000";
            this.tbDragY1.Name = "tbDragY1";
            this.tbDragY1.Size = new System.Drawing.Size(26, 23);
            this.tbDragY1.TabIndex = 2;
            this.tbDragY1.ValidatingType = typeof(int);
            this.tbDragY1.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDragY1.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "To";
            // 
            // tbDragX2
            // 
            this.tbDragX2.Location = new System.Drawing.Point(140, 22);
            this.tbDragX2.Mask = "000";
            this.tbDragX2.Name = "tbDragX2";
            this.tbDragX2.Size = new System.Drawing.Size(26, 23);
            this.tbDragX2.TabIndex = 4;
            this.tbDragX2.ValidatingType = typeof(int);
            this.tbDragX2.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbDragX2.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // gbClick
            // 
            this.gbClick.Controls.Add(this.label1);
            this.gbClick.Controls.Add(this.tbPointX);
            this.gbClick.Controls.Add(this.tbPointY);
            this.gbClick.Enabled = false;
            this.gbClick.Location = new System.Drawing.Point(3, 3);
            this.gbClick.Name = "gbClick";
            this.gbClick.Size = new System.Drawing.Size(245, 56);
            this.gbClick.TabIndex = 9;
            this.gbClick.TabStop = false;
            this.gbClick.Text = "Point";
            this.gbClick.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Here";
            // 
            // tbPointX
            // 
            this.tbPointX.Location = new System.Drawing.Point(49, 22);
            this.tbPointX.Mask = "000";
            this.tbPointX.Name = "tbPointX";
            this.tbPointX.Size = new System.Drawing.Size(26, 23);
            this.tbPointX.TabIndex = 1;
            this.tbPointX.ValidatingType = typeof(int);
            this.tbPointX.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbPointX.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // tbPointY
            // 
            this.tbPointY.Location = new System.Drawing.Point(83, 22);
            this.tbPointY.Mask = "000";
            this.tbPointY.Name = "tbPointY";
            this.tbPointY.Size = new System.Drawing.Size(25, 23);
            this.tbPointY.TabIndex = 2;
            this.tbPointY.TextChanged += new System.EventHandler(this.AllFields_TextChanged);
            this.tbPointY.Enter += new System.EventHandler(this.MaskedTextBox_Enter);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "json";
            this.openFileDialog1.FileName = "botConfig.json";
            this.openFileDialog1.Filter = "json files|*.json|All files|*.*";
            this.openFileDialog1.Title = "Bot Config";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "json";
            this.saveFileDialog1.Filter = "json files|*.json|All files|*.*";
            // 
            // cmsFindString
            // 
            this.cmsFindString.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFindStringToolStripMenuItem1,
            this.toolStripMenuItem13,
            this.deleteToolStripMenuItem1});
            this.cmsFindString.Name = "cmsFindString";
            this.cmsFindString.Size = new System.Drawing.Size(157, 54);
            // 
            // addFindStringToolStripMenuItem1
            // 
            this.addFindStringToolStripMenuItem1.Name = "addFindStringToolStripMenuItem1";
            this.addFindStringToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.addFindStringToolStripMenuItem1.Text = "Add Find String";
            this.addFindStringToolStripMenuItem1.Click += new System.EventHandler(this.AddFindStringtoolStripMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(153, 6);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // cmsAction
            // 
            this.cmsAction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addActionToolStripMenuItem1,
            this.addCommandToolStripMenuItem1,
            this.toolStripSeparator1,
            this.pasteToolStripMenuItem2,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem2});
            this.cmsAction.Name = "cmsAction";
            this.cmsAction.Size = new System.Drawing.Size(157, 104);
            this.cmsAction.Opening += new System.ComponentModel.CancelEventHandler(this.cms_Opening);
            // 
            // addActionToolStripMenuItem1
            // 
            this.addActionToolStripMenuItem1.Name = "addActionToolStripMenuItem1";
            this.addActionToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.addActionToolStripMenuItem1.Text = "Add Action";
            this.addActionToolStripMenuItem1.Click += new System.EventHandler(this.AddActionToolStripMenuItem_Click);
            // 
            // addCommandToolStripMenuItem1
            // 
            this.addCommandToolStripMenuItem1.Name = "addCommandToolStripMenuItem1";
            this.addCommandToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.addCommandToolStripMenuItem1.Text = "Add Command";
            this.addCommandToolStripMenuItem1.Click += new System.EventHandler(this.AddCommandToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // pasteToolStripMenuItem2
            // 
            this.pasteToolStripMenuItem2.Enabled = false;
            this.pasteToolStripMenuItem2.Name = "pasteToolStripMenuItem2";
            this.pasteToolStripMenuItem2.Size = new System.Drawing.Size(156, 22);
            this.pasteToolStripMenuItem2.Text = "Paste";
            this.pasteToolStripMenuItem2.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 6);
            // 
            // deleteToolStripMenuItem2
            // 
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(156, 22);
            this.deleteToolStripMenuItem2.Text = "Delete";
            this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // cmsBasicCommand
            // 
            this.cmsBasicCommand.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertAboveToolStripMenuItem,
            this.insertBelowToolStripMenuItem,
            this.toolStripMenuItem10,
            this.moveUpToolStripMenuItem1,
            this.moveDownToolStripMenuItem1,
            this.toolStripMenuItem11,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1,
            this.toolStripMenuItem12,
            this.deleteToolStripMenuItem4});
            this.cmsBasicCommand.Name = "cmsBasicCommand";
            this.cmsBasicCommand.Size = new System.Drawing.Size(141, 176);
            this.cmsBasicCommand.Opening += new System.ComponentModel.CancelEventHandler(this.cms_Opening);
            // 
            // insertAboveToolStripMenuItem
            // 
            this.insertAboveToolStripMenuItem.Name = "insertAboveToolStripMenuItem";
            this.insertAboveToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.insertAboveToolStripMenuItem.Text = "Insert Above";
            this.insertAboveToolStripMenuItem.Click += new System.EventHandler(this.AboveToolStripMenuItem_Click);
            // 
            // insertBelowToolStripMenuItem
            // 
            this.insertBelowToolStripMenuItem.Name = "insertBelowToolStripMenuItem";
            this.insertBelowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.insertBelowToolStripMenuItem.Text = "Insert Below";
            this.insertBelowToolStripMenuItem.Click += new System.EventHandler(this.BelowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(137, 6);
            // 
            // moveUpToolStripMenuItem1
            // 
            this.moveUpToolStripMenuItem1.Name = "moveUpToolStripMenuItem1";
            this.moveUpToolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.moveUpToolStripMenuItem1.Text = "Move Up";
            this.moveUpToolStripMenuItem1.Click += new System.EventHandler(this.UpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem1
            // 
            this.moveDownToolStripMenuItem1.Name = "moveDownToolStripMenuItem1";
            this.moveDownToolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.moveDownToolStripMenuItem1.Text = "Move Down";
            this.moveDownToolStripMenuItem1.Click += new System.EventHandler(this.DownToolStripMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(137, 6);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Enabled = false;
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.pasteToolStripMenuItem1.Text = "Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(137, 6);
            // 
            // deleteToolStripMenuItem4
            // 
            this.deleteToolStripMenuItem4.Name = "deleteToolStripMenuItem4";
            this.deleteToolStripMenuItem4.Size = new System.Drawing.Size(140, 22);
            this.deleteToolStripMenuItem4.Text = "Delete";
            this.deleteToolStripMenuItem4.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // cmsContainingCommand
            // 
            this.cmsContainingCommand.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCommandToolStripMenuItem2,
            this.toolStripMenuItem6,
            this.insertAboveToolStripMenuItem1,
            this.insertBelowToolStripMenuItem1,
            this.toolStripMenuItem7,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripMenuItem8,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem9,
            this.deleteToolStripMenuItem3});
            this.cmsContainingCommand.Name = "cmsContainingCommand";
            this.cmsContainingCommand.Size = new System.Drawing.Size(157, 204);
            this.cmsContainingCommand.Opening += new System.ComponentModel.CancelEventHandler(this.cms_Opening);
            // 
            // addCommandToolStripMenuItem2
            // 
            this.addCommandToolStripMenuItem2.Name = "addCommandToolStripMenuItem2";
            this.addCommandToolStripMenuItem2.Size = new System.Drawing.Size(156, 22);
            this.addCommandToolStripMenuItem2.Text = "Add Command";
            this.addCommandToolStripMenuItem2.Click += new System.EventHandler(this.AddCommandToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(153, 6);
            // 
            // insertAboveToolStripMenuItem1
            // 
            this.insertAboveToolStripMenuItem1.Name = "insertAboveToolStripMenuItem1";
            this.insertAboveToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.insertAboveToolStripMenuItem1.Text = "Insert Above";
            this.insertAboveToolStripMenuItem1.Click += new System.EventHandler(this.AboveToolStripMenuItem_Click);
            // 
            // insertBelowToolStripMenuItem1
            // 
            this.insertBelowToolStripMenuItem1.Name = "insertBelowToolStripMenuItem1";
            this.insertBelowToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.insertBelowToolStripMenuItem1.Text = "Insert Below";
            this.insertBelowToolStripMenuItem1.Click += new System.EventHandler(this.BelowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(153, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.UpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.DownToolStripMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(153, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(153, 6);
            // 
            // deleteToolStripMenuItem3
            // 
            this.deleteToolStripMenuItem3.Name = "deleteToolStripMenuItem3";
            this.deleteToolStripMenuItem3.Size = new System.Drawing.Size(156, 22);
            this.deleteToolStripMenuItem3.Text = "Delete";
            this.deleteToolStripMenuItem3.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1689, 655);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScriptEditor";
            this.Text = "ScriptEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptEditor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbEnterText.ResumeLayout(false);
            this.gbEnterText.PerformLayout();
            this.gbLoops.ResumeLayout(false);
            this.gbLoops.PerformLayout();
            this.gbList.ResumeLayout(false);
            this.gbList.PerformLayout();
            this.gbImageArea.ResumeLayout(false);
            this.gbImageArea.PerformLayout();
            this.gbActionOverride.ResumeLayout(false);
            this.gbActionOverride.PerformLayout();
            this.gbAppName.ResumeLayout(false);
            this.gbAppName.PerformLayout();
            this.gbPickAction.ResumeLayout(false);
            this.gbPickAction.PerformLayout();
            this.gbAppControl.ResumeLayout(false);
            this.gbAppControl.PerformLayout();
            this.gbWFNC.ResumeLayout(false);
            this.gbWFNC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWFNCDetectPercent)).EndInit();
            this.gbFindText.ResumeLayout(false);
            this.gbFindText.PerformLayout();
            this.gbAction.ResumeLayout(false);
            this.gbAction.PerformLayout();
            this.gbSleep.ResumeLayout(false);
            this.gbSleep.PerformLayout();
            this.gbImageNames.ResumeLayout(false);
            this.gbImageNames.PerformLayout();
            this.gbImageNameAndWait.ResumeLayout(false);
            this.gbImageNameAndWait.PerformLayout();
            this.gbImageName.ResumeLayout(false);
            this.gbImageName.PerformLayout();
            this.gbLoopCoordinate.ResumeLayout(false);
            this.gbLoopCoordinate.PerformLayout();
            this.gbDrag.ResumeLayout(false);
            this.gbDrag.PerformLayout();
            this.gbClick.ResumeLayout(false);
            this.gbClick.PerformLayout();
            this.cmsFindString.ResumeLayout(false);
            this.cmsAction.ResumeLayout(false);
            this.cmsBasicCommand.ResumeLayout(false);
            this.cmsContainingCommand.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvBotData;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MaskedTextBox tbDragY2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox tbDragX2;
        private System.Windows.Forms.MaskedTextBox tbDragY1;
        private System.Windows.Forms.MaskedTextBox tbDragX1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox tbPointY;
        private System.Windows.Forms.MaskedTextBox tbPointX;
        private System.Windows.Forms.GroupBox gbDrag;
        private System.Windows.Forms.GroupBox gbClick;
        private System.Windows.Forms.GroupBox gbLoopCoordinate;
        private System.Windows.Forms.RadioButton rbLoopCoordY;
        private System.Windows.Forms.RadioButton rbLoopCoordX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbImageName;
        private System.Windows.Forms.GroupBox gbImageNameAndWait;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox tbTimeout;
        private System.Windows.Forms.GroupBox gbSleep;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox tbDelay;
        private System.Windows.Forms.GroupBox gbImageNames;
        private System.Windows.Forms.ListBox lbImageNames;
        private System.Windows.Forms.ComboBox cbImageNameNoWait;
        private System.Windows.Forms.ComboBox cbImageNameWithWait;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnRemoveImageNames;
        private System.Windows.Forms.Button btnAddImageNames;
        private System.Windows.Forms.ComboBox cbImageNamesForList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MaskedTextBox tbImageNamesWait;
        private System.Windows.Forms.GroupBox gbAction;
        private System.Windows.Forms.MaskedTextBox tbActionFrequency;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbFindText;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.MaskedTextBox tbFindTextBackTolerance;
        private System.Windows.Forms.MaskedTextBox tbFindTextTextTolerance;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.MaskedTextBox tbFindTextSearchX1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MaskedTextBox tbFindTextSearchY2;
        private System.Windows.Forms.MaskedTextBox tbFindTextSearchY1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.MaskedTextBox tbFindTextSearchX2;
        private System.Windows.Forms.TextBox tbFindTextSearch;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbFindTextName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbActionName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox gbWFNC;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.MaskedTextBox tbWFNCWait;
        private System.Windows.Forms.MaskedTextBox tbWFNCX1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.MaskedTextBox tbWFNCY2;
        private System.Windows.Forms.MaskedTextBox tbWFNCY1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.MaskedTextBox tbWFNCX2;
        private System.Windows.Forms.NumericUpDown nudWFNCDetectPercent;
        private System.Windows.Forms.GroupBox gbAppControl;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.MaskedTextBox tbAppControlWait;
        private System.Windows.Forms.TextBox tbAppControlName;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.DateTimePicker dtpActionTimeOfDay;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cbActionType;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox gbPickAction;
        private System.Windows.Forms.ComboBox cbPickActionAction;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox gbAppName;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbAppNameAppId;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.MaskedTextBox tbAppNameTimeout;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem belowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox gbActionOverride;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.DateTimePicker dtpActionOverrideLastRun;
        private System.Windows.Forms.CheckBox tbActionOverrideEnabled;
        private System.Windows.Forms.DateTimePicker dtptbActionOverrideTimeOfDay;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbActionOverrideName;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.MaskedTextBox tbActionOverrideFrequency;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.GroupBox gbImageArea;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ListBox lbImageAreaAreas;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ComboBox cbImageAreasImage;
        private System.Windows.Forms.MaskedTextBox tbImageAreasX;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.MaskedTextBox tbImageAreasH;
        private System.Windows.Forms.MaskedTextBox tbImageAreasY;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.MaskedTextBox tbImageAreasW;
        private System.Windows.Forms.Button btImageAreaRemove;
        private System.Windows.Forms.Button btImageAreaAdd;
        private System.Windows.Forms.GroupBox gbList;
        private System.Windows.Forms.TextBox tbListName;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Button btnPastFindText;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFindStringtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCoordinatestoolStripMenuItem;
        private System.Windows.Forms.Button btnFindTextGenerate;
        private System.Windows.Forms.ToolStripMenuItem addCommandToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbIgnoreMissing;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.MaskedTextBox tbDragTime;
        private System.Windows.Forms.ComboBox cbActionAfter;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.ComboBox cbActionBefore;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.GroupBox gbLoops;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.MaskedTextBox tbLoopsCounter;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbImageNameMissingOk;
        private System.Windows.Forms.CheckBox cbImageNamesMissingOk;
        private System.Windows.Forms.TextBox tbLoopsOverrideId;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ToolTip scriptEditorToolTip;
        private System.Windows.Forms.ListView lvActionOverridesOverride;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TextBox tbActionOverrideValue;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Button btActionOverridesRemove;
        private System.Windows.Forms.Button btActionOverridesAdd;
        private System.Windows.Forms.TextBox tbActionOverrideOverrideId;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btActionOverridesEdit;
        private System.Windows.Forms.GroupBox gbEnterText;
        private System.Windows.Forms.TextBox tbEnterTextText;
        private System.Windows.Forms.TextBox tbEnterTextOverideId;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.ContextMenuStrip cmsFindString;
        private System.Windows.Forms.ToolStripMenuItem addFindStringToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip cmsAction;
        private System.Windows.Forms.ToolStripMenuItem addActionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addCommandToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip cmsBasicCommand;
        private System.Windows.Forms.ToolStripMenuItem insertAboveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertBelowToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsContainingCommand;
        private System.Windows.Forms.ToolStripMenuItem addCommandToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem insertAboveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertBelowToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem2;
    }
}

