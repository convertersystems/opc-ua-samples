// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Workstation.ServiceModel.Ua;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Workstation.MobileHmi
{
    public class App : Application
    {
        private string discoveryUrl = @"opc.tcp://10.0.2.2:26543"; // Use ip address ( or '10.0.2.2' for accessing local host computer from emulator. hostname, localhost or 127.0.0.1 will not work on Android emu!)

        private ILoggerFactory loggerFactory;
        private ILogger logger;
        private UaTcpSessionClient session;

        protected override void OnStart()
        {
            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
            this.loggerFactory.AddDebug(LogLevel.Trace);
            this.logger = this.loggerFactory.CreateLogger<App>();

            // Create the session client for the app.
            this.session = new UaTcpSessionClient(
                new ApplicationDescription()
                {
                    ApplicationName = "Workstation.MobileHmi",
                    ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.MobileHmi",
                    ApplicationType = ApplicationType.Client
                },
                new DirectoryStore(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pki"), loggerFactory: this.loggerFactory),
                this.ProvideUserIdentity,
                this.discoveryUrl,
                this.loggerFactory);

            // Show the MainPage
            var viewModel = new MainPageViewModel(this.session);
            var view = new MainPage { BindingContext = viewModel };

            this.MainPage = new NavigationPage(view);
        }

        protected override void OnSleep()
        {
            this.session?.SuspendAsync().Wait();
        }

        protected override void OnResume()
        {
            this.session?.Resume();
        }

        /// <summary>
        /// Shows a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        private Task<IUserIdentity> ProvideUserIdentity(EndpointDescription endpoint)
        {
            // Due to problem with dns on android emulator, the endpoint url's hostname is rewritten with an ip address.
            endpoint.EndpointUrl = this.discoveryUrl;

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                return Task.FromResult<IUserIdentity>(new UserNameIdentity("root", "secret"));
            }

            return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
        }
    }
}
