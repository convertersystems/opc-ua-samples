// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using RobotHmi.Services;
using RobotHmi.Views;
using Workstation.ServiceModel.Ua;

namespace RobotHmi
{
    public class MainModule : IModule
    {
        private IRegionManager regionManager;
        private IUnityContainer container;

        public MainModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
        }

        public void Initialize()
        {
            // Register shared services with the application's dependency injection container.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotHmi",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotHmi",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var discoveryUrl = Properties.Settings.Default.PLC1ServiceUrl;
            var plc1Service = new PLC1Service(appDescription, appCertificate, null, discoveryUrl);
            this.container.RegisterInstance(plc1Service, new ContainerControlledLifetimeManager());

            // Add the module's views to the application's navigation structure.
            this.container.RegisterTypeForNavigation<MainView>("RobotHmi.Views.MainView");
            this.container.RegisterTypeForNavigation<DetailView>("RobotHmi.Views.DetailView");
            this.container.RegisterTypeForNavigation<AxisView>("RobotHmi.Views.AxisView");

            // Navigate to the main view.
            this.regionManager.RequestNavigate(RegionNames.MainContent, "RobotHmi.Views.MainView");
        }
    }
}