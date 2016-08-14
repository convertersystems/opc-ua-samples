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
        private UaTcpSessionClient session;

        protected override void OnStartup(StartupEventArgs e)
        {
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.StatusHmi",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.StatusHmi",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var endpointUrl = StatusHmi.Properties.Settings.Default.EndpointUrl;
            this.session = new UaTcpSessionClient(appDescription, appCertificate, null, endpointUrl);
            this.Resources["Session"] = this.session;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.session?.Dispose();
            base.OnExit(e);
        }
    }
}
