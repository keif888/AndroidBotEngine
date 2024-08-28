// <copyright file="Program.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using CommandLine;

namespace BotEngine
{
    partial class Program
    {
        public class Options : IGameOptions, IListOptions
        {
            public string DeviceString { get; set; }

            public string ConfigFileName { get; set; }

            public string ListConfigFileName { get; set; }

            public string DeviceFileName { get; set; }

            public int SleepTime { get; set; }

            public bool ListDevices { get; set; }

            [Option('p', "ADBPath", Required = true, HelpText = "The path to where you have ADB.exe")]
            public string ADBPath { get; set; }

            public string LogLevel { get; set; }

            /// <summary>
            /// Used to hold the name of the process that the emulator is running in
            /// </summary>
            public string ProcessName { get; set; }

            /// <summary>
            /// Used to hold the name of the main window to identify which instance of the ProcessName to attach to
            /// </summary>
            public string WindowName { get; set; }

            public int TopLeftX { get; set; }
            public int TopLeftY { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }

        }
    }
}
