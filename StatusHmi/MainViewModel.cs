// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Step 1: Add the following namespaces.
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// A model for MainWindow.
    /// </summary>
    public class MainViewModel : ViewModelBase, ISubscription // Step 2: Add base class and interface of type ISubscription.
    {
        public MainViewModel()
        {
            this.Session = App.Current.Session; // Step 4: Set the six properties that implement ISubscription.
            this.PublishingInterval = 1000.0;
            this.KeepAliveCount = 10;
            this.LifetimeCount = 0;
            this.PublishingEnabled = true;
            this.MonitoredItems = new MonitoredItemCollection(this);
            this.Session.Subscribe(this); // Step 5: Subscribe for data change and event notifications.
        }

        // Step 3: Add these six properties that implement ISubscription.
        public UaTcpSessionClient Session { get; }

        public double PublishingInterval { get; }

        public uint KeepAliveCount { get; }

        public uint LifetimeCount { get; }

        public bool PublishingEnabled { get; }

        public MonitoredItemCollection MonitoredItems { get; }

        /// <summary>
        /// Gets the value of ServerServerStatus.
        /// </summary>
        [MonitoredItem(nodeId: "i=2256")] // Step 6: Add a [MonitoredItem] attribute.
        public ServerStatusDataType ServerServerStatus
        {
            get { return this.serverServerStatus; }
            private set { this.SetProperty(ref this.serverServerStatus, value); }
        }

        private ServerStatusDataType serverServerStatus;
    }
}
