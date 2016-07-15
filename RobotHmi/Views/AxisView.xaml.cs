// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using RobotHmi.ViewModels;

namespace RobotHmi.Views
{
    /// <summary>
    /// A view for an Axis.
    /// </summary>
    public sealed partial class AxisView : IRegionMemberLifetime, INavigationAware
    {
        public AxisView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets a value indicating whether this view and viewmodel are kept alive while in the navigation history.
        /// </summary>
        public bool KeepAlive => false;

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Read the NavigationParameters passed by the caller.
            var name = navigationContext.Parameters["displayName"] as string;
            switch (name)
            {
                case "Axis 1":
                    this.DataContext = ServiceLocator.Current.GetInstance<Axis1ViewModel>();
                    break;
                case "Axis 2":
                    this.DataContext = ServiceLocator.Current.GetInstance<Axis2ViewModel>();
                    break;
                case "Axis 3":
                    this.DataContext = ServiceLocator.Current.GetInstance<Axis3ViewModel>();
                    break;
                case "Axis 4":
                    this.DataContext = ServiceLocator.Current.GetInstance<Axis4ViewModel>();
                    break;

                default:
                    break;
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.DataContext = null; // allows viewmodel to be collected.
        }
    }
}