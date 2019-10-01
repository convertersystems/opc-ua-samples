// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prism.Ioc;
using RobotHmi.Views;
using Workstation.ServiceModel.Ua;

namespace RobotHmi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        private UaApplication application;

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup a logger.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddDebug());
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // Read 'appSettings.json' for endpoint configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true)
                .Build();

            // Build and run an OPC UA application instance.
            this.application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:Workstation.RobotHmi")
                .SetDirectoryStore(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Workstation.RobotHmi",
                    "pki"))
                .SetIdentity(this.ShowSignInDialog)
                .SetLoggerFactory(loggerFactory)
                .AddMappedEndpoints(config)
                .Build();

            this.application.Run();

            base.OnStartup(e);
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs e)
        {
            this.application?.Dispose();
        }

        /// <summary>
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// Requires MainWindow to derive from MahApps.Metro.Controls.MetroWindow.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        private Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
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

                        LoginDialogSettings loginSettings = new LoginDialogSettings { InitialUsername = initialUserName, UsernameWatermark = "root", PasswordWatermark = "secret" };
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView>("RobotHmi.Views.MainView");
            containerRegistry.RegisterForNavigation<DetailView>("RobotHmi.Views.DetailView");
            containerRegistry.RegisterForNavigation<AxisView>("RobotHmi.Views.AxisView");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }
    }
}