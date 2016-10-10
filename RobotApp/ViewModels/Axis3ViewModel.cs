// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Template10.Mvvm;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A view model for Axis3.
    /// </summary>
    [Subscription(publishingInterval: 500, keepAliveCount: 20)]
    public class Axis3ViewModel : ViewModelBase, IAxisViewModel
    {
        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.Set(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 3";
    }
}