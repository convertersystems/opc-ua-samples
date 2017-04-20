// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Workstation.ServiceModel.Ua;
using Xamarin.Forms;

namespace Workstation.MobileHmi
{
    public class App : Application
    {
        private string discoveryUrl = @"opc.tcp://10.0.2.2:26543"; // Use ip address ( or '10.0.2.2' for accessing local host computer from emulator. hostname, localhost or 127.0.0.1 will not work on Android emu!)

        private ILoggerFactory loggerFactory;
        private UaApplication application;

        protected override void OnStart()
        {
            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
            this.loggerFactory.AddDebug(LogLevel.Trace);

            // Build and run an OPC UA application instance.
            this.application = new UaApplicationBuilder()
                .UseApplicationUri($"urn:localhost:Workstation.MobileHmi")
                .UseDirectoryStore(@"%HOME%/.local/share/pki")
                .UseIdentityProvider(this.ShowSignInDialog)
                .UseLoggerFactory(this.loggerFactory)
                .AddEndpoint("PLC1", this.discoveryUrl)
                .Build();

            this.application.Run();

            // Show the MainPage
            this.MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnSleep()
        {
            this.application?.SuspendAsync().Wait();
        }

        protected override void OnResume()
        {
            this.application?.Run();
        }

        /// <summary>
        /// Shows a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        private async Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return new AnonymousIdentity();
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                return new UserNameIdentity("root", "secret");
            }

            return new AnonymousIdentity();
        }
    }
}
