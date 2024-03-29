# Android Bot Engine

Download [here](https://github.com/keif888/AndroidBotEngine/releases)

## Project Description

This project aims to produce an engine and editor that can drive Android devices or emulators via ADB (Android Debug Bridge) from a windows console app.  
It will support configurable Actions, made up of a set of coded Commands.  
The console app will receive a set of 3 json files that contain the configuration to drive an app on the Android device.  
The json files are:  
1. GameConfig
	- This file has all the Image Recognition strings, and the Actions that have been configured
1. ListConfig
	- This file has the lists of lists of locations to do things at
1. DeviceConfig
	- This file has the list of Actions that the device has done, when it last did them, and can be used to enable/disable Actions

The Editor will support the 3 configuration files.  To date, it has only view capability for the GameConfig file.


## Supported Commands

The list of commands to date are:

1. Click
	- Sends a click at the specifed screen coordinates
1. Drag
	- Sends a swipe from one screen coordinate location to another screen coordinate location
1. ClickWhenNotFoundInArea
	- Searches a list of rectangular areas for a specified image, and when not found, clicks on the 1st area that didn't have the image
1. Exit
	- Exits from a configurable Action
1. LoopCoordinates
	- Takes the name of a list of coordinates (from the ListConfig json file), and passes them into the EnterLoopCoordinate command
	- Supports nested Commands
1. EnterLoopCoordinate
	- Takes the specified X/Y coordinate from a configured list (via Command LoopCoordinates) and sends them to the Android Device as keyboard data entry
1. EnterText
    - Takes a string, and sends it to the Android Device as keyboard data entry
1. FindClick
	- Searches for a specified image, and clicks on it when found
1. FindClickAndWait
	- Searches for a specified image, and clicks on it when found, and then waits for it to be removed from the screen
1. IfExists
	- Searches for a specified image, and when found initates the nested Commands
	- Supports nested Commands
1. IfNotExists
	- Searches for a specified image, and when *NOT* found initates the nested Commands
	- Supports nested Commands
1. LoopUntilFound
	- Searches for a specified image, and when *NOT* found initates the nested Commands
	- Repeat the search and nested commands until the image is found
	- If the timeout (configurable) interval is exceeded, exit the Action
	- Supports nested Commands
1. LoopUntilNotFound
	- Searches for a specified image, and when found initates the nested Commands
	- Repeat the search and nested commands until the image is *NOT* found
	- If the timeout (configurable) interval is exceeded, exit the Action
	- Supports nested Commands
1. Restart
	- Restarts the Action
1. RunAction
	- Executes the named Action
1. Sleep
	- Wait for the defined number of milliseconds
1. StartGame
	- Starts the Android App that is configured
1. StopGame
	- Force Stop's the Android App that is configured
1. WaitFor
	- Waits for the configured timeout period until the specified image appears on screen
	- If the timeout (configurable) interval is exceeded, exit the Action
1. WaitForThenClick
	- Waits for the configured timeout period until the specified image appears on screen
	- If the image appears, click on it
	- If the timeout (configurable) interval is exceeded, exit the Action
1. WaitForChange
	- Watches a configurable area on the screen for changes that exceed a configurable percentage
	- If changes are detected, subsequent Commands in the Action will be run
	- If the timeout (configurable) interval is exceeded, exit the Action
1. WaitForNoChange
	- Watches a configurable area on the screen for changes that exceed a configurable percentage
	- If *NO* changes are detected, subsequent Commands in the Action will be run
	- If the timeout (configurable) interval is exceeded, exit the Action


## Release Notes
2022-10-19 - 0.5.5.110_beta release.  
Fix colour graphic image detection  
Improve non bot interaction handling  
Fix issue with not Zeroing counters when starting an action  
Fix incorrect detection of json for paste operation  
2022-09-16 - 0.5.4.104_beta release.  
LDPlayer version 4.0.83 tested and working  
Adds detection of non bot interaction with controlled device, which will then wait until 30 seconds has passed from last non bot interaction before resuming bot actions  
Adds right click menu options in editor  
Changes default for all actions to disabled on 1st run against a new device config file  
Add colour graphic image capture to editor and ability to find within bot  
Fix FindClickAndWait command to work correctly on a list of images  
Fix a number of usability issues in editor  
2022-08-10 - 0.5.4.66_beta released.  
Known issues with LDPlayer versions 4.0.76.1, 4.0.78, 4.0.79 and 4.0.81 due to ADB retrieval of FrameBuffer being unstable  
LDPlayer versions 4.0.76 and 4.0.83 tested and working  
Adds Enable and Disable keyboard commands, and updates Adhoc keyboard command to allow changing Override settings  
2022-08-07 - 0.4.4.63_beta released.  
Known issues with LDPlayer versions greater than 4.0.76 due to ADB retrieval of FrameBuffer being unstable  
Adds EnterText command, ability within bot engine to start Adhoc Actions (hit A on the keyboard)  
2022-05-13 - 0.3.1.14_beta released.  
Known issues with LDPlayer versions greater than 4.0.76 due to ADB retrieval of FrameBuffer being unstable  
