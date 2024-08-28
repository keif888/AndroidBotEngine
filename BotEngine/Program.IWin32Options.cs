// <copyright file="Program.IWin32Options.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace BotEngine
{
    partial class Program
    {
        interface IWin32Options
        {

            [Option('p', "ProcessName", SetName = "Win32Capture", Required = true, HelpText = "The name of the process that the emulator is running in (without the .exe).")]
            public string ProcessName { get; set; }

            [Option('n', "WindowName", SetName = "Win32Capture", Required = true, HelpText = "The name of the Main Window that the emulator is running.  Check the MultiPlayer for your emulator.")]
            public string WindowName { get; set; }

            [Option('x', "TopLeftX", SetName = "Win32Capture", Required = false, HelpText = "The top left X offset to read from the emulator window", Default = 1)]
            public int TopLeftX { get; set; }

            [Option('y', "TopLeftY", SetName = "Win32Capture", Required = false, HelpText = "The top left Y offset to read from the emulator window", Default = 34)]
            public int TopLeftY { get; set; }

            [Option('H', "Height", SetName = "Win32Capture", Required = false, HelpText = "The Height of the area to read from the emulator window", Default = 540)]
            public int Height { get; set; }

            [Option('W', "Width", SetName = "Win32Capture", Required = false, HelpText = "The Width of the area read from the emulator window", Default = 960)]
            public int Width { get; set; }

        }
    }
}
