// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Threading;

namespace RobotHmi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        private static readonly MetroLog.ILogger Log;
        private AppBootstrapper bootstrapper;

        static App()
        {
            MetroLog.LogManagerFactory.DefaultConfiguration = new MetroLog.LoggingConfiguration();
#if DEBUG
            MetroLog.LogManagerFactory.DefaultConfiguration.AddTarget(MetroLog.LogLevel.Trace, MetroLog.LogLevel.Fatal, new MetroLog.Targets.DebugTarget());
#else
            MetroLog.LogManagerFactory.DefaultConfiguration.AddTarget(MetroLog.LogLevel.Info, MetroLog.LogLevel.Fatal, new MetroLog.Targets.StreamingFileTarget());
#endif
            Log = MetroLog.LogManagerFactory.DefaultLogManager.GetLogger<App>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            this.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif
            this.bootstrapper = new AppBootstrapper();
            this.bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.bootstrapper.Dispose();
        }

#if !DEBUG

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            if (ex != null)
            {
                Log.Error(ex.Message, ex);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Log.Error(ex.Message, ex);
            }

            if (!e.IsTerminating)
            {
                MessageBox.Show(RobotHmi.Properties.Resources.UnhandledException, RobotHmi.Properties.Resources.ApplicationError, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
#endif
    }
}