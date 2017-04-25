// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using RobotHmi.Views;

namespace RobotHmi
{
    /// <summary>
    /// The module that initializes the MainView and required services.
    /// </summary>
    public class MainModule : IModule
    {
        private IRegionManager regionManager;
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="container">The Unity container.</param>
        public MainModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            // Register the views with the container using the navigation string.
            this.container.RegisterTypeForNavigation<MainView>("RobotHmi.Views.MainView");
            this.container.RegisterTypeForNavigation<DetailView>("RobotHmi.Views.DetailView");
            this.container.RegisterTypeForNavigation<AxisView>("RobotHmi.Views.AxisView");

            // Navigate to the main view.
            this.regionManager.RequestNavigate(RegionNames.MainContent, "RobotHmi.Views.MainView");
        }
    }
}