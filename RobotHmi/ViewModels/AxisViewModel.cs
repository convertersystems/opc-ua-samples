// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Mvvm;
using Prism.Regions;
using RobotHmi.Services;
using System;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for AxisView. Shows how to pass parameters to the view model.
    /// </summary>
    /// <remarks>
    /// In this example, a parameter is passed in the navigation url
    /// that indicates the particular axis to display.
    /// </remarks>
    public class AxisViewModel : NavigationAwareSubscriptionBase
    {
        private PLC1Service service;

        public AxisViewModel(PLC1Service service)
        {
            this.service = service;
            this.PublishingInterval = 250;
            this.KeepAliveCount = 40;
        }

        /// <summary>
        /// Gets the value of the Axis.
        /// </summary>
        public float Axis
        {
            get { return this.axis; }
            private set { this.SetProperty(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
            private set { this.SetProperty(ref this.displayName, value); }
        }

        private string displayName;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Read the NavigationParameters passed by the caller.
            var name = navigationContext.Parameters["displayName"] as string;
            switch (name)
            {
                case "Axis 1":
                    this.DisplayName = "Axis 1";
                    this.MonitoredItems.Add(new MonitoredItem { NodeId = NodeId.Parse("ns=2;s=Robot1_Axis1"), Property = this.GetType().GetProperty(nameof(this.Axis)) });
                    break;
                case "Axis 2":
                    this.DisplayName = "Axis 2";
                    this.MonitoredItems.Add(new MonitoredItem { NodeId = NodeId.Parse("ns=2;s=Robot1_Axis2"), Property = this.GetType().GetProperty(nameof(this.Axis)) });
                    break;
                case "Axis 3":
                    this.DisplayName = "Axis 3";
                    this.MonitoredItems.Add(new MonitoredItem { NodeId = NodeId.Parse("ns=2;s=Robot1_Axis3"), Property = this.GetType().GetProperty(nameof(this.Axis)) });
                    break;
                case "Axis 4":
                    this.DisplayName = "Axis 4";
                    this.MonitoredItems.Add(new MonitoredItem { NodeId = NodeId.Parse("ns=2;s=Robot1_Axis4"), Property = this.GetType().GetProperty(nameof(this.Axis)) });
                    break;

                default:
                    break;
            }

            this.service.Subscriptions.Add(this);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.service.Subscriptions.Remove(this);
            this.MonitoredItems.Clear();
        }
    }
}