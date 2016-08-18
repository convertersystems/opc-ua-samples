// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.Services;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A view model for Axis1.
    /// </summary>
    public class Axis1ViewModel : SubscriptionBase, IAxisViewModel
    {
        public Axis1ViewModel(PLC1Service session)
        {
            this.PublishingInterval = 500.0;
            session?.Subscribe(this);
        }

        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.Set(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 1";
    }
}