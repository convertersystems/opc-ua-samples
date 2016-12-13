// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Workstation.ServiceModel.Ua;

namespace RobotHmi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        private AppBootstrapper bootstrapper;
        private ILoggerFactory loggerFactory;

        /// <summary>
        /// Gets the current instance of the local application.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="ApplicationDescription"/> of the local application.
        /// </summary>
        public ApplicationDescription ApplicationDescription { get; } = new ApplicationDescription
        {
            ApplicationName = "Workstation.RobotHmi",
            ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotHmi",
            ApplicationType = ApplicationType.Client
        };

        /// <summary>
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// Requires MainWindow to derive from MahApps.Metro.Controls.MetroWindow.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        public Task<IUserIdentity> ProvideUserIdentity(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                this.Dispatcher.InvokeAsync(
                    async () =>
                    {
                        var shell = (MetroWindow)this.MainWindow;
                        var userNamesDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(RobotHmi.Properties.Settings.Default.UserNames) ?? new Dictionary<string, string>();
                        var userNameKey = $"userName_{endpoint.EndpointUrl}";

                        var initialUserName = string.Empty;
                        if (userNamesDictionary.ContainsKey(userNameKey))
                        {
                            initialUserName = userNamesDictionary[userNameKey];
                        }

                        LoginDialogSettings loginSettings = new LoginDialogSettings { InitialUsername = initialUserName };
                        var result = await shell.ShowLoginAsync("SIGN IN", $"Connecting to server '{endpoint.Server.ApplicationName}' at '{endpoint.EndpointUrl}'.", loginSettings);
                        if (result != null && !string.IsNullOrEmpty(result.Username))
                        {
                            userNamesDictionary[userNameKey] = result.Username;
                            RobotHmi.Properties.Settings.Default.UserNames = JsonConvert.SerializeObject(userNamesDictionary);
                            RobotHmi.Properties.Settings.Default.Save();

                            tcs.TrySetResult(new UserNameIdentity(result.Username, result.Password));
                        }
                        tcs.TrySetResult(new AnonymousIdentity());
                    },
                    DispatcherPriority.ApplicationIdle);
                return tcs.Task;
            }

            throw new NotImplementedException("ProvideUserIdentity supports only UserName and Anonymous identity, for now.");
        }

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
#if DEBUG
            this.loggerFactory.AddDebug(LogLevel.Trace);
#else
            this.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif

            this.bootstrapper = new AppBootstrapper(this.loggerFactory);
            this.bootstrapper.Run();
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs e)
        {
            this.bootstrapper?.Dispose();
            this.loggerFactory?.Dispose();
        }

#if !DEBUG

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            if (ex != null)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Trace.TraceError(ex.Message);
            }

            if (!e.IsTerminating)
            {
                MessageBox.Show(RobotHmi.Properties.Resources.UnhandledException, RobotHmi.Properties.Resources.ApplicationError, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
#endif
    }
}