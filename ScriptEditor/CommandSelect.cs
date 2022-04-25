using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static BotEngineClient.BotEngine;

namespace ScriptEditor
{
    public partial class CommandSelect : Form
    {
        public CommandSelect()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Returns the command that was selected.
        /// </summary>
        public ValidCommandIds SelectedCommand { get; private set; }

        private void lbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCommand = Enum.Parse<ValidCommandIds>(lbCommands.SelectedItem.ToString());
            switch (SelectedCommand)
            {
                case ValidCommandIds.Click:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
\viewkind4\uc1\pard\sa200\sl276\slmult1\b\f0\fs32\lang9 Click\par\b0
\fs22 This command will send a finger tap to the coordinates that you will provide.\par\par}";
                    break;
                case ValidCommandIds.ClickWhenNotFoundInArea:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
\viewkind4\uc1\pard\sa200\sl276\slmult1\b\f0\fs32\lang9 Click When Not Found In Area\par\b0
\fs22 This command will search for a selected FindString within a set of areas that you will define.  The first area found without the FindString will be clicked.\par\par}
";
                    break;
                case ValidCommandIds.Drag:
                    break;
                case ValidCommandIds.Exit:
                    break;
                case ValidCommandIds.EnterLoopCoordinate:
                    break;
                case ValidCommandIds.FindClick:
                    break;
                case ValidCommandIds.FindClickAndWait:
                    break;
                case ValidCommandIds.IfExists:
                    break;
                case ValidCommandIds.IfNotExists:
                    break;
                case ValidCommandIds.LoopCoordinates:
                    break;
                case ValidCommandIds.LoopUntilFound:
                    break;
                case ValidCommandIds.LoopUntilNotFound:
                    break;
                case ValidCommandIds.Restart:
                    break;
                case ValidCommandIds.RunAction:
                    break;
                case ValidCommandIds.Sleep:
                    break;
                case ValidCommandIds.StartGame:
                    break;
                case ValidCommandIds.StopGame:
                    break;
                case ValidCommandIds.WaitFor:
                    break;
                case ValidCommandIds.WaitForThenClick:
                    break;
                case ValidCommandIds.WaitForChange:
                    break;
                case ValidCommandIds.WaitForNoChange:
                    break;
                default:
                    break;
            }

        }
    }
}
