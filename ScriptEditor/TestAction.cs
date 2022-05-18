// <copyright file="TestAction.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BotEngineClient;
using SharpAdbClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Action = BotEngineClient.Action;
using System.Threading;

namespace ScriptEditor
{
    public partial class TestAction : Form
    {
        private BOTConfig botGameConfig;
        private AdbServer server;
        private BOTListConfig botListConfig;
        private ServiceCollection services;
        private readonly ILogger _logger;
        private TextBoxWriter? writer;
        private BotEngine.CommandResults threadResult;
        private CancellationTokenSource threadCTS;

        public static IServiceProvider ServiceProvider { get; set; }

        public TestAction()
        {
            InitializeComponent();
            writer = new TextBoxWriter(tbLogger);
            Console.SetOut(writer);
            services = new ServiceCollection();
            ServiceProvider = ConfigureServices();
            _logger = (ILogger)ServiceProvider.GetService(typeof(ILogger));
            threadCTS = null;
        }

        public void LoadConfig(BOTConfig gameConfig, BOTListConfig listConfig)
        {
            botGameConfig = gameConfig;
            botListConfig = listConfig;
            cbActions.Items.Clear();
            foreach (KeyValuePair<string, Action> item in gameConfig.SystemActions)
            {
                cbActions.Items.Add(item.Key);
            }
            foreach (KeyValuePair<string, Action> item in gameConfig.Actions)
            {
                cbActions.Items.Add(item.Key);
            }
            cbActions.SelectedIndex = 0;
            Console.WriteLine("Is This Captured?");
        }


        public IServiceProvider ConfigureServices()
        {
            LogLevel logLevel = LogLevel.Debug;
            if (logLevel == LogLevel.Debug)
            {
                services.AddLogging(loggingBuilder => loggingBuilder
                  .AddSimpleConsole(options => {
                      options.IncludeScopes = true;
                      options.SingleLine = false;
                      options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffffff ";
                      options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                  })
                  .AddDebug()
                  .SetMinimumLevel(logLevel)
                  );
            }
            else
            {
                services.AddLogging(loggingBuilder => loggingBuilder
                .AddSimpleConsole(options => {
                    options.IncludeScopes = true;
                    options.SingleLine = false;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffffff ";
                    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                })
                 .SetMinimumLevel(logLevel)
                );
            }
            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("AndroidBot"));
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Initiates an action, logging to the text box in debug mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTest_Click(object sender, EventArgs e)
        {
            Action action = null;
            tbLogger.Text = string.Empty;
            string actionName = cbActions.SelectedItem.ToString();
            if (botGameConfig.SystemActions.ContainsKey(actionName))
            {
                action = botGameConfig.SystemActions[actionName];
            }
            else if (botGameConfig.Actions.ContainsKey(actionName))
            {
                action = botGameConfig.Actions[actionName];
            }
            else
            {
                MessageBox.Show("Invalid Action Selected.  Try Again.", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (action != null)
            {
                if (server == null)
                {
                    server = new AdbServer();
                    StartServerResult result = server.StartServer(AppDomain.CurrentDomain.BaseDirectory + @"\ADB\adb.exe", restartServerIfNewer: true);
                    if (result != StartServerResult.AlreadyRunning)
                    {
                        Thread.Sleep(1500);
                        AdbServerStatus status = server.GetStatus();
                        if (!status.IsRunning)
                        {
                            MessageBox.Show("Unable to start ADB server");
                            return;
                        }
                    }
                }

                AdbClient client = new AdbClient();
                List<DeviceData> devices = client.GetDevices();

                List<string> devicesList = new List<string>();
                DeviceSelect deviceSelect = new DeviceSelect();
                foreach (DeviceData device in devices)
                {
                    string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                    string deviceId = string.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
                    devicesList.Add(deviceId);
                }
                deviceSelect.LoadList(devicesList);
                if (deviceSelect.ShowDialog() == DialogResult.OK)
                {
                    string deviceId = deviceSelect.selectedItem;

                    BotEngineClient.BotEngine bot = new BotEngineClient.BotEngine(ServiceProvider, AppDomain.CurrentDomain.BaseDirectory + @"\ADB\adb.exe", deviceId, botGameConfig.FindStrings, botGameConfig.SystemActions, botGameConfig.Actions, botListConfig);
                    BotEngine.CommandResults cr;
                    threadCTS = new CancellationTokenSource();
                    //ToDo: Make the logging from these responsive, instead of character by character.
                    bool threadActive = false;
                    Thread botThread;
                    if (action.BeforeAction != null)
                    {
                        bot.SetThreadingCommand(action.BeforeAction, ResultCallback);
                        botThread = new Thread(bot.InitiateThreadingCommand);
                        botThread.Start(threadCTS.Token);
                        threadActive = true;
                        while (threadActive)
                        {
                            threadActive = !botThread.Join(50);
                            Application.DoEvents();
                        }
                        //cr = bot.ExecuteAction(action.BeforeAction);
                        cr = threadResult;
                        if (cr == BotEngineClient.BotEngine.CommandResults.ADBError  || cr == BotEngine.CommandResults.Cancelled)
                        {
                            return;
                        }
                    }
                    bot.SetThreadingCommand(actionName, ResultCallback);
                    botThread = new Thread(bot.InitiateThreadingCommand);
                    botThread.Start(threadCTS.Token);
                    threadActive = true;
                    while (threadActive)
                    {
                        threadActive = !botThread.Join(50);
                        Application.DoEvents();
                    }
                    cr = threadResult;
                    // cr = bot.ExecuteAction(actionName);
                    _logger.LogInformation(string.Format("Action result was {0}", cr));
                    if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngine.CommandResults.Cancelled)
                    {
                        return;
                    }
                    if (action.AfterAction != null)
                    {
                        bot.SetThreadingCommand(action.AfterAction, ResultCallback);
                        botThread = new Thread(bot.InitiateThreadingCommand);
                        botThread.Start(threadCTS.Token);
                        threadActive = true;
                        while (threadActive)
                        {
                            threadActive = !botThread.Join(50);
                            Application.DoEvents();
                        }
                        cr = threadResult;
                        //cr = bot.ExecuteAction(action.AfterAction);
                        if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngine.CommandResults.Cancelled)
                        {
                            return;
                        }
                    }
                }
            }
        }

        public void ResultCallback(BotEngine.CommandResults result)
        {
            threadResult = result;
        }

        private void TestAction_Load(object sender, EventArgs e)
        {
            //writer = new TextBoxWriter(tbLogger);
            //Console.SetOut(writer);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (threadCTS != null)
            {
                threadCTS.Cancel();
            }
        }
    }
}
