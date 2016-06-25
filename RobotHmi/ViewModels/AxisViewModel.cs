// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    /// In this example, two query parameters are passed in the navigation url:
    /// nodeId := a string representation of the nodeId to monitor.
    /// displayName := a string to display in the view.
    /// </remarks>
    public class AxisViewModel : NavigationAwareSubscriptionBase
    {

        public AxisViewModel(PLC1Service service)
            : base(service)
        {
            this.PublishingInterval = 250;
            this.KeepAliveCount = 40;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Read the NavigationParameters passed by the caller.
            this.AxisNodeId = navigationContext.Parameters["nodeId"] as string;
            this.AxisDisplayName = navigationContext.Parameters["displayName"] as string;

            // Alter the NodeId of the MonitoredItem for property 'Axis'.
            this.MonitoredItems[nameof(this.Axis)].NodeId = NodeId.Parse(this.AxisNodeId);

            // Calling base will initiate subscribing.
            base.OnNavigatedTo(navigationContext);
        }

        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem]
        public float Axis
        {
            get { return this.axisField; }
            private set { this.SetProperty(ref this.axisField, value); }
        }

        private float axisField;

        /// <summary>
        /// Gets the NodeId.
        /// </summary>
        public string AxisNodeId
        {
            get { return this.axisNodeId; }
            private set { this.SetProperty(ref this.axisNodeId, value); }
        }

        private string axisNodeId;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string AxisDisplayName
        {
            get { return this.axisDisplayName; }
            private set { this.SetProperty(ref this.axisDisplayName, value); }
        }

        private string axisDisplayName;
    }
}