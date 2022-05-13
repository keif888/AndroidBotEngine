// <copyright file="FileTypeSelect.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using BotEngineClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class FileTypeSelect : Form
    {
        /// <summary>
        /// Stores the radio button that was selected.
        /// </summary>
        public JsonHelper.ConfigFileType ConfigFileType { get; private set; }
        public FileTypeSelect()
        {
            InitializeComponent();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDeviceConfig.Checked)
                ConfigFileType = JsonHelper.ConfigFileType.DeviceConfig;
            else if (rbGameConfig.Checked)
                ConfigFileType = JsonHelper.ConfigFileType.GameConfig;
            else
                ConfigFileType = JsonHelper.ConfigFileType.ListConfig;
        }
    }
}
