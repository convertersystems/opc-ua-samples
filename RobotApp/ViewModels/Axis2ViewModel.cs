// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.Services;
using Template10.Mvvm;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A view model for Axis2.
    /// </summary>
    public class Axis2ViewModel : ViewModelBase, ISubscription, IAxisViewModel
    {
        public Axis2ViewModel(PLC1Service session)
        {
            this.Session = session;
            this.PublishingInterval = 500.0;
            this.KeepAliveCount = 20;
            this.LifetimeCount = 0;
            this.PublishingEnabled = true;
            this.MonitoredItems = new MonitoredItemCollection(this);
            this.Session.Subscribe(this);
        }

        public UaTcpSessionClient Session { get; }

        public double PublishingInterval { get; }

        public uint KeepAliveCount { get; }

        public uint LifetimeCount { get; }

        public bool PublishingEnabled { get; }

        public MonitoredItemCollection MonitoredItems { get; }

        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.Set(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 2";
    }
}