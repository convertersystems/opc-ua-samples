// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using System.Windows;
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// A model for MainWindow.
    /// </summary>
    public class MainViewModel : ViewModelBase, ISubscription
    {
        public MainViewModel()
        {
            this.Session = (UaTcpSessionClient)Application.Current.Resources["Session"];
            this.PublishingInterval = 1000.0;
            this.KeepAliveCount = 10;
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
        /// Invokes the method ServerGetMonitoredItems.
        /// </summary>
        /// <param name="inArgs">The input arguments.</param>
        /// <returns>A <see cref="Task"/> that returns the output arguments.</returns>
        public async Task<object[]> ServerGetMonitoredItems(params object[] inArgs)
        {
            var response = await this.Session.CallAsync(new CallRequest
            {
                MethodsToCall = new[]
                {
                    new CallMethodRequest
                    {
                        ObjectId = NodeId.Parse("i=2253"),
                        MethodId = NodeId.Parse("i=11492"),
                        InputArguments = inArgs.ToVariantArray()
                    }
                }
            });

            var result = response.Results[0];
            if (StatusCode.IsBad(result.StatusCode))
            {
                throw new ServiceResultException(new ServiceResult(result.StatusCode));
            }

            return result.OutputArguments.ToObjectArray();
        }

        /// <summary>
        /// Gets the value of ServerServerStatus.
        /// </summary>
        [MonitoredItem(nodeId: "i=2256")]
        public ServerStatusDataType ServerServerStatus
        {
            get { return this.serverServerStatus; }
            private set { this.SetProperty(ref this.serverServerStatus, value); }
        }

        private ServerStatusDataType serverServerStatus;
    }
}
