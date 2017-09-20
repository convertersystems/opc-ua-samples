// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Workstation.ServiceModel.Ua;
using Xamarin.Forms;

namespace Workstation.MobileHmi
{
    public class App : Xamarin.Forms.Application
    {
        private ILoggerFactory loggerFactory;
        private UaApplication application;

        protected override void OnStart()
        {
            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
            this.loggerFactory.AddDebug(LogLevel.Trace);

            // Build and run an OPC UA application instance.
            this.application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:Workstation.MobileHmi")
                .SetDirectoryStore(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "pki"))
                .SetIdentity(this.ShowSignInDialog)
                .SetLoggerFactory(this.loggerFactory)
                .AddMappedEndpoint("opc.tcp://localhost:26543", "opc.tcp://10.0.2.2:26543")
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
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        public async Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return new AnonymousIdentity();
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var vm = new LoginPageViewModel { Endpoint = endpoint };
                if (Xamarin.Forms.Application.Current.Properties.ContainsKey("UserName"))
                {
                    vm.UserName = Xamarin.Forms.Application.Current.Properties["UserName"] as string;
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var v = new LoginPage { BindingContext = vm };
                    await this.MainPage.Navigation.PushModalAsync(v);
                    await vm.Task;
                    await this.MainPage.Navigation.PopModalAsync();
                    Xamarin.Forms.Application.Current.Properties["UserName"] = vm.UserName;
                });

                return await vm.Task;
            }

            throw new NotImplementedException("ProvideUserIdentity supports only UserName and Anonymous identity, for now.");
        }
    }
}
