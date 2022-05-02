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

        private void LbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCommand = Enum.Parse<ValidCommandIds>(lbCommands.SelectedItem.ToString());
            switch (SelectedCommand)
            {
                case ValidCommandIds.Click:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\f0\fs32\lang9 Click\par
\b0\fs22 This command will send a finger tap to the coordinates that you will provide.\par
}";
                    break;
                case ValidCommandIds.ClickWhenNotFoundInArea:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\f0\fs32\lang9 Click When Not Found In Area\par
\b0\fs22 This command will search for a selected FindString within a set of areas that you will define.  The first area found without the FindString will be clicked.\par\par
}";
                    break;
                case ValidCommandIds.Drag:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Drag\par
\b0\fs22 This command will send a finger tap to the coordinates that you will provide.\par
}";
                    break;
                case ValidCommandIds.Exit:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Exit\par
\b0\fs22 This command will stop the current action, and mark it completed.\par
}";
                    break;
                case ValidCommandIds.EnterLoopCoordinate:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Enter Loop Coordinate\par
\b0\fs22 This command will enter the text associated with the coordinate part (X/Y) from the list selected via \b Loop Coordiantes\b0 .\par
}";
                    break;
                case ValidCommandIds.FindClick:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Find Click\par
\b0\fs22 This command will search for a specified Image, and then click onhte centre of it if found.\par
}";
                    break;
                case ValidCommandIds.FindClickAndWait:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Find Click and Wait\par
\b0\fs22 This command will search for a specified Image, and then click onhte centre of it if found.  It will then wait for the button to be removed from the screen.  If this wait times out (configurable) then the Action will be marked as failed, and retried.\par
}";
                    break;
                case ValidCommandIds.IfExists:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 If Exists\par
\b0\fs22 This command will search for an Image, and if found, execute the commands that are configured within it.\par
}";
                    break;
                case ValidCommandIds.IfNotExists:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 If Not Exists\par
\b0\fs22 This command will search for an Image, and if NOT found, execute the commands that are configured within it.\par
}";
                    break;
                case ValidCommandIds.LoopCoordinates:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Loop Coordinates\par
\b0\fs22 This command loop through all the coordinates in a list, which is configured in the List Config file.  All the commands that are configured within it, will be run for each set of coordinates in the list.  The \b Enter Loop Coordinate\b0  command is used to type the coordinates into the Android phone.\par
}";
                    break;
                case ValidCommandIds.LoopUntilFound:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Loop Until Found\par
\b0\fs22 This command will run a set of commands configured within it, until a configured Image shows up on the phone.  The search for the image is done each time before the set of commands are executed.  If the image is found, the commands are not executed.\par
}";
                    break;
                case ValidCommandIds.LoopUntilNotFound:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Loop Until Not Found\par
\b0\fs22 This command will run a set of commands configured within it, until a configured Image disappears from  the phone.  The search for the image is done each time before the set of commands are executed.  If the image is found, the commands are executed.\par
}";
                    break;
                case ValidCommandIds.Restart:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Restart\par
\b0\fs22 This command will stop the current Action, and leave it marked as incomplete.  It will then start again.\par
}";
                    break;
                case ValidCommandIds.RunAction:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Run Action\par
\b0\fs22 This command will start the named action.\par
}";
                    break;
                case ValidCommandIds.Sleep:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Sleep\par
\b0\fs22 This command will pause the execution of the Action for the number of milli seconds entered.\par
}";
                    break;
                case ValidCommandIds.StartGame:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Start Game\par
\b0\fs22 This command will start the Android Application specified.\par
To find the list of packages available on your device, use the ADB command:\par
\f1 ADB shell p list packages \par
\f0 Once you know the name of the package then edit the following command to get the main activity.  You may need to guess based on the output.\par
\f1 adb shell\par
dumpsys package | grep -Eo ""^[[:space:]]+[0-9a-f]+[[:space:]]+\highlight1\b com.android.chrome\highlight0\b0 /[^[:space:]]+"" | grep -oE ""[^[:space:]]+$""\f0\par
Finally enter the command like this into the editor (which is to launch the chrome browser:\par
\f1 com.android.chrome/com.google.android.apps.chrome.Main\f0\par
";
                    break;
                case ValidCommandIds.StopGame:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Stop Game\par
\b0\fs22 This command will stop the game specified.  See Start Game for how to specify the game.\par
}";
                    break;
                case ValidCommandIds.WaitFor:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Wait For\par
\b0\fs22 This command will wait until the selected image appears on the screen.  If that takes more than the specified time, then the Action will be taken as failed, and scheduled for retry.\par
}";
                    break;
                case ValidCommandIds.WaitForThenClick:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Wait For Then Click\par
\b0\fs22 This command will wait until the selected image appears on the screen, and then click in the middle of it.  If that takes more than the specified time, then the Action will be taken as failed, and scheduled for retry.\par
}";
                    break;
                case ValidCommandIds.WaitForChange:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Wait for Change\par
\b0\fs22 This command will wait for the contents within an area of the screen to change more than the specified percentage.  If that takes more than the specified time, then the Action will be taken as failed, and scheduled for retry.\par
}";
                    break;
                case ValidCommandIds.WaitForNoChange:
                    rtbCommandHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang3081{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\sa200\sl276\slmult1
\b\fs32 Wait for No Change\par
\b0\fs22 This command will wait for the contents within an area of the screen to change more than the specified percentage.  If changes occur within the specified time, then the Action will be taken as failed, and scheduled for retry.\par
}";
                    break;
                default:
                    break;
            }

        }
    }
}
