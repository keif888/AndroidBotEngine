// <copyright file="Program.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using CommandLine;

namespace BotEngine
{
    partial class Program
    {
        interface IListOptions
        {
            [Option('t', "ListDevices", SetName = "ListOptions", Required = true, HelpText = "Returns a list of all the devices available.")]
            bool ListDevices { get; set; }

        }
    }
}
