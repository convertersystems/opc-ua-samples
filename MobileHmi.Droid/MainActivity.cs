// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.Logging;
using Workstation.ServiceModel.Ua;

namespace Workstation.MobileHmi
{
    [Activity(Label = "Workstation.MobileHmi", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        private string discoveryUrl = @"opc.tcp://10.0.2.2:26543"; // Use ip address ( or '10.0.2.2' for accessing local host computer from emulator. hostname, localhost or 127.0.0.1 will not work on Android emu!)

        private ILoggerFactory loggerFactory;
        private ILogger logger;
        private UaTcpSessionClient session;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
            this.loggerFactory.AddDebug(LogLevel.Trace);
            this.logger = this.loggerFactory.CreateLogger<MainActivity>();

            try
            {
                // Create the session client for the app.
                this.session = new UaTcpSessionClient(
                    new ApplicationDescription()
                    {
                        ApplicationName = "Workstation.MobileHmi",
                        ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.MobileHmi",
                        ApplicationType = ApplicationType.Client
                    },
                    new DirectoryStore(
                        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "pki"),
                        loggerFactory: loggerFactory),
                    this.ProvideUserIdentity,
                    this.discoveryUrl,
                    this.loggerFactory);

                this.LoadApplication(new App(this.session));
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error creating UaTcpSessionClient. {0}", ex.Message);
            }
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