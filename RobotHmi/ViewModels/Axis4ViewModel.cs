// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotHmi.Services;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for Axis4.
    /// </summary>
    public class Axis4ViewModel : ViewModelBase, ISubscription, IAxisViewModel
    {
        public Axis4ViewModel(PLC1Service session)
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
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.SetProperty(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 4";
    }
}