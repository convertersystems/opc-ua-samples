// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using RobotHmi.Views;
using Workstation.ServiceModel.Ua;
using RobotHmi.ViewModels;
using RobotHmi.Services;

namespace RobotHmi
{
    /// <summary>
    /// The module that initializes the MainView and required services.
    /// </summary>
    public class MainModule : IModule
    {
        private IRegionManager regionManager;
        private IUnityContainer container;

        public MainModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            // Prepare for constructing the shared PLC1Session.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotHmi",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotHmi",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var userIdentityProvider = new Func<EndpointDescription, Task<IUserIdentity>>(((Shell)Application.Current.MainWindow).ProvideUserIdentity);
            var session = new PLC1Session(appDescription, appCertificate, userIdentityProvider);

            // Register the shared services with the application's dependency injection container.
            this.container.RegisterInstance(session);

            // Register the views with the container using the navigation string.
            this.container.RegisterTypeForNavigation<MainView>("RobotHmi.Views.MainView");
            this.container.RegisterTypeForNavigation<DetailView>("RobotHmi.Views.DetailView");
            this.container.RegisterTypeForNavigation<AxisView>("RobotHmi.Views.AxisView");

            // Navigate to the main view.
            this.regionManager.RequestNavigate(RegionNames.MainContent, "RobotHmi.Views.MainView");
        }
    }
}