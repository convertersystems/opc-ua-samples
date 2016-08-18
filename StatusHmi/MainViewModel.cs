// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Step 1: Add the following namespaces.
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// A model for MainWindow.
    /// </summary>
    public class MainViewModel : SubscriptionBase // Step 2: Add your subscription base class (which implements ISubscription and INotifyPropertyChanged).
    {
        public MainViewModel()
        {
            this.PublishingInterval = 1000.0; // Step 3: Adjust the publishing interval (in ms.) here.
            App.Current.Session.Subscribe(this); // Step 4: Subscribe for data change and event notifications.
        }

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
