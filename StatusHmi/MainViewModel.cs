// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Step 1: Add the following namespaces.
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// A model for MainView.
    /// </summary>
    [Subscription(publishingInterval: 500, keepAliveCount: 20)] // Step 2: Add a [Subscription] attribute.
    public class MainViewModel : ViewModelBase // Step 3: Add your base class (which implements INotifyPropertyChanged).
    {
        /// <summary>
        /// Gets the value of ServerServerStatus.
        /// </summary>
        [MonitoredItem(nodeId: "i=2256")] // Step 4: Add a [MonitoredItem] attribute.
        public ServerStatusDataType ServerServerStatus
        {
            get { return this.serverServerStatus; }
            private set { this.SetProperty(ref this.serverServerStatus, value); }
        }

        private ServerStatusDataType serverServerStatus;
    }
}
