// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Read the axis name from the NavigationParameters passed by the caller.
            var name = navigationContext.Parameters["displayName"] as string;

            // Create the correct view model, based on the axis name.
            switch (name)
            {
                case "Axis 1":
                    this.DataContext = new Axis1ViewModel();
                    break;
                case "Axis 2":
                    this.DataContext = new Axis2ViewModel();
                    break;
                case "Axis 3":
                    this.DataContext = new Axis3ViewModel();
                    break;
                case "Axis 4":
                    this.DataContext = new Axis4ViewModel();
                    break;
            }
        }

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            (this.DataContext as IDisposable)?.Dispose();
            this.DataContext = null;
        }
    }
}