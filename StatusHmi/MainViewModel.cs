﻿// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua; // Install-Package Workstation.UaClient

namespace StatusHmi
{
    /// <summary>
    /// A model for MainView.
    /// </summary>
    [Subscription(endpointUrl: "opc.tcp://localhost:48010", publishingInterval: 500, keepAliveCount: 20)]
    public class MainViewModel : SubscriptionBase
    {
        /// <summary>
        /// Gets the value of ServerServerStatus.
        /// </summary>
        [MonitoredItem(nodeId: "i=2256")]
        public ServerStatusDataType ServerServerStatus
        {
            get { return this._serverServerStatus; }
            private set { this.SetProperty(ref _serverServerStatus, value); }
        }

        private ServerStatusDataType _serverServerStatus;
    }
}
