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
            _logger.BeginScope("TestAction");
            threadCTS = null;
        }

        /// <summary>
        /// Loads the config from the caller.
        /// Must be executed before Showing the form.
        /// </summary>
        /// <param name="gameConfig"></param>
        /// <param name="listConfig"></param>
        public void SetupTestActionForm(BOTConfig gameConfig, BOTListConfig listConfig, List<string> devicesList)
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
            cbDevices.Items.Clear();
            foreach(string item in devicesList)
            {
                cbDevices.Items.Add(item);
            }
            cbDevices.SelectedIndex = 0;
        }

        /// <summary>
        /// Configures the logging services that the bot uses.  Set to Debug to capture as much as possible.
        /// </summary>
        /// <returns></returns>
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
                      options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Disabled;
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
                    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Disabled;
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
                btnTest.Enabled = false;
                btnCancel.Enabled = true;
                WorkerArgument workerArgument = new WorkerArgument {
                    action = action,
                    actionName = actionName,
                    deviceId = cbDevices.SelectedItem.ToString()
                };
                // Execute Action on background thread.
                testWorker.RunWorkerAsync(workerArgument);
            }
        }

        /// <summary>
        /// Event that gets fired when the background worker is run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = ExecuteTestAction(e.Argument as WorkerArgument);
        }

        /// <summary>
        /// Fires when the background worker thread has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnTest.Enabled = true;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// Executes the test action within a background worker thread.
        /// </summary>
        /// <param name="workerArgument"></param>
        /// <returns></returns>
        private BotEngine.CommandResults ExecuteTestAction(WorkerArgument workerArgument)
        {
            Action action = workerArgument.action;
            string deviceId = workerArgument.deviceId;
            string actionName = workerArgument.actionName;

            BotEngineClient.BotEngine bot = new BotEngineClient.BotEngine(ServiceProvider, AppDomain.CurrentDomain.BaseDirectory + @"\ADB\adb.exe", deviceId, botGameConfig.FindStrings, botGameConfig.SystemActions, botGameConfig.Actions, botListConfig);
            BotEngine.CommandResults cr;
            threadCTS = new CancellationTokenSource();
            Thread botThread;
            if (action.BeforeAction != null)
            {
                bot.SetThreadingCommand(action.BeforeAction, ResultCallback);
                botThread = new Thread(bot.InitiateThreadingCommand);
                botThread.Start(threadCTS.Token);
                botThread.Join();
                cr = threadResult;
                if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngine.CommandResults.Cancelled)
                {
                    return cr;
                }
            }
            bot.SetThreadingCommand(actionName, ResultCallback);
            botThread = new Thread(bot.InitiateThreadingCommand);
            botThread.Start(threadCTS.Token);
            botThread.Join();
            cr = threadResult;
            _logger.LogInformation(string.Format("Action result was {0}", cr));
            if (cr == BotEngineClient.BotEngine.CommandResults.ADBError || cr == BotEngine.CommandResults.Cancelled)
            {
                return cr;
            }
            if (action.AfterAction != null)
            {
                bot.SetThreadingCommand(action.AfterAction, ResultCallback);
                botThread = new Thread(bot.InitiateThreadingCommand);
                botThread.Start(threadCTS.Token);
                botThread.Join();
                cr = threadResult;
                return cr;
            }
            return BotEngine.CommandResults.Exit;
        }

        /// <summary>
        /// Call back from a bot thread with the results of the bot action.
        /// </summary>
        /// <param name="result"></param>
        public void ResultCallback(BotEngine.CommandResults result)
        {
            threadResult = result;
        }

        /// <summary>
        /// Send a cancel instruction to the bot thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (threadCTS != null)
            {
                threadCTS.Cancel();
            }
        }


    }

    /// <summary>
    /// Class to pass values to the Background Worker Thread.
    /// </summary>
    public class WorkerArgument
    {
        public string deviceId;
        public string actionName;
        public Action action;
    }
}
