// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the current App object.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets or sets the default session for the app.
        /// </summary>
        public UaTcpSessionClient Session { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Describe this app.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.StatusHmi",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.StatusHmi",
                ApplicationType = ApplicationType.Client
            };

            // Get (or create) the app's X509Certificate
            var appCertificate = appDescription.GetCertificate();

            // Get the remote endpoint from the app.config
            var endpointUrl = StatusHmi.Properties.Settings.Default.EndpointUrl;

            // Create the default session for the app.
            this.Session = new UaTcpSessionClient(appDescription, appCertificate, null, endpointUrl);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Session?.Dispose();
            base.OnExit(e);
        }
    }
}
