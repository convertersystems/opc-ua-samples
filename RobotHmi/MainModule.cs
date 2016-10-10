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
            // Prepare for constructing the shared UaTcpSessionClient.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotHmi",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotHmi",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var userIdentityProvider = new Func<UaTcpSessionClient, Task<IUserIdentity>>(((Shell)Application.Current.MainWindow).ProvideUserIdentity);
            var discoveryUrl = Properties.Settings.Default.PLC1DiscoveryUrl;

            // Register the shared services with the application's dependency injection container.
            this.container.RegisterInstance("PLC1", new UaTcpSessionClient(appDescription, appCertificate, userIdentityProvider, discoveryUrl));

            // Register the subscriptions with the container using a factory method.
            this.container.RegisterType<MainViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<MainViewModel>()));
            this.container.RegisterType<Axis1ViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis1ViewModel>()));
            this.container.RegisterType<Axis2ViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis2ViewModel>()));
            this.container.RegisterType<Axis3ViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis3ViewModel>()));
            this.container.RegisterType<Axis4ViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis4ViewModel>()));

            // Register the views with the container using the navigation string.
            this.container.RegisterTypeForNavigation<MainView>("RobotHmi.Views.MainView");
            this.container.RegisterTypeForNavigation<DetailView>("RobotHmi.Views.DetailView");
            this.container.RegisterTypeForNavigation<AxisView>("RobotHmi.Views.AxisView");

            // Navigate to the main view.
            this.regionManager.RequestNavigate(RegionNames.MainContent, "RobotHmi.Views.MainView");
        }
    }
}