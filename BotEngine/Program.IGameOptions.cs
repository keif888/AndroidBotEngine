// <copyright file="Program.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using CommandLine;

namespace BotEngine
{
    partial class Program
    {
        interface IGameOptions
        {
            [Option('d', "Device", SetName = "GameOptions", Required = false, HelpText = "The device string that is returned by ADB.exe.  At least the following is required \"emulator-5556 device\".")]
            string DeviceString { get; set; }

            [Option('g', "GameConfig", SetName = "GameOptions", Required = true, HelpText = "The name of the json file that contains the Search Strings and Actions.")]
            string ConfigFileName { get; set; }

            [Option('i', "ListConfig", SetName = "GameOptions", Required = true, HelpText = "The name of the json file that contains the List Settings.")]
            string ListConfigFileName { get; set; }

            [Option('c', "DeviceConfig", SetName = "GameOptions", Required = true, HelpText = "The name of the json file that contains the last time actions were taken for a device.  If the file doesn't exist it will be created.")]
            string DeviceFileName { get; set; }

            [Option('s', "SleepTime", SetName = "GameOptions", Required = false, HelpText = "The number of milliseconds to sleep between each loop of Actions.", Default = 500)]
            int SleepTime { get; set; }

            [Option('l', "LogLevel", Required = false, HelpText = "The level of output from the logger (None, Critical, Error, Warning, Information, Debug, Trace).", Default = "Warning")]
            public string LogLevel { get; set; }

            [Option('P', "ProcessName", Required = true, HelpText = "The name of the process that the emulator is running in (without the .exe).")]
            public string ProcessName { get; set; }

            [Option('n', "WindowName", Required = true, HelpText = "The name of the Main Window that the emulator is running.  Check the MultiPlayer for your emulator.")]
            public string WindowName { get; set; }

            [Option('x', "TopLeftX", Required = false, HelpText = "The top left X offset to read from the emulator window", Default = 1)]
            public int TopLeftX { get; set; }

            [Option('y', "TopLeftY", Required = false, HelpText = "The top left Y offset to read from the emulator window", Default = 34)]
            public int TopLeftY { get; set; }

            [Option('H', "Height", Required = false, HelpText = "The Height of the area to read from the emulator window", Default = 960)]
            public int Height { get; set; }

            [Option('W', "Width", Required = false, HelpText = "The Width of the area read from the emulator window", Default = 540)]
            public int Width { get; set; }
        }
    }
}
