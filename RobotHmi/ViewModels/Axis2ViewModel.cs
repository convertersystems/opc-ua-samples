// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotHmi.Services;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for Axis2.
    /// </summary>
    public class Axis2ViewModel : SubscriptionBase, IAxisViewModel
    {
        public Axis2ViewModel(PLC1Service session)
        {
            this.PublishingInterval = 500.0;
            session?.Subscribe(this);
        }

        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.SetProperty(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 2";
    }
}