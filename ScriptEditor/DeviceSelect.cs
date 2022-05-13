// <copyright file="DeviceSelect.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScriptEditor
{
    /// <summary>
    /// Form to show a list of Android devices (found with ADB), and allow one of them to be selected.
    /// </summary>
    public partial class DeviceSelect : Form
    {
        /// <summary>
        /// Layout the form, and clear the selected item.
        /// </summary>
        public DeviceSelect()
        {
            InitializeComponent();
            selectedItem = string.Empty;
        }

        /// <summary>
        /// Returns the text of the selected Android Device
        /// </summary>
        public string selectedItem { get; private set; }

        /// <summary>
        /// Allows a List of strings to be passed in, and loads it into the List Box on the form
        /// </summary>
        /// <param name="items"></param>
        public void LoadList(List<string> items)
        {
            lbDevices.Items.Clear();
            foreach (string item in items)
            {
                lbDevices.Items.Add(item);
            }
        }

        /// <summary>
        /// Populates the selectedItem Property when an item is selected in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDevices.SelectedItems.Count > 0)
                btnOk.Enabled = true;
            selectedItem = (string)lbDevices.SelectedItem;
        }
    }
}
