// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A base class for Subscriptions.
    /// </summary>
    public class SubscriptionBase : ViewModelBase, ISubscription
    {
        public SubscriptionBase()
        {
            // Set default values for a subscription.
            this.PublishingInterval = 1000.0;
            this.KeepAliveCount = 20;
            this.LifetimeCount = 0;
            this.PublishingEnabled = true;
            this.MonitoredItems = new MonitoredItemCollection(this);
        }

        public UaTcpSessionClient Session { get; set; }

        public double PublishingInterval { get; set; }

        public uint KeepAliveCount { get; set; }

        public uint LifetimeCount { get; set; }

        public bool PublishingEnabled { get; set; }

        public MonitoredItemCollection MonitoredItems { get; set; }
    }
}
