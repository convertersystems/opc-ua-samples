// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// A base class for Subscriptions.
    /// </summary>
    public class SubscriptionBase : ViewModelBase, ISubscription, INotifyDataErrorInfo
    {
        private UaTcpSessionClient session;
        private double publishingInterval;
        private uint keepAliveCount;
        private uint lifetimeCount;
        private bool publishingEnabled;
        private MonitoredItemCollection monitoredItems;
        private ErrorsContainer<string> errors;

        public SubscriptionBase()
        {
            // Set default values for a subscription.
            this.PublishingInterval = 1000.0;
            this.KeepAliveCount = 20;
            this.LifetimeCount = 0;
            this.PublishingEnabled = true;
            this.MonitoredItems = new MonitoredItemCollection(this);
            this.errors = new ErrorsContainer<string>(p => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p)));
        }

        public UaTcpSessionClient Session
        {
            get { return this.session; }
            set { this.SetProperty(ref this.session, value); }
        }

        public double PublishingInterval
        {
            get { return this.publishingInterval; }
            set { this.SetProperty(ref this.publishingInterval, value); }
        }

        public uint KeepAliveCount
        {
            get { return this.keepAliveCount; }
            set { this.SetProperty(ref this.keepAliveCount, value); }
        }

        public uint LifetimeCount
        {
            get { return this.lifetimeCount; }
            set { this.SetProperty(ref this.lifetimeCount, value); }
        }

        public bool PublishingEnabled
        {
            get { return this.publishingEnabled; }
            set { this.SetProperty(ref this.publishingEnabled, value); }
        }

        public MonitoredItemCollection MonitoredItems
        {
            get { return this.monitoredItems; }
            set { this.SetProperty(ref this.monitoredItems, value); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get { return this.errors.HasErrors; }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return this.errors.GetErrors(propertyName);
        }

        public void SetErrors(string propertyName, IEnumerable<string> errors)
        {
            this.errors.SetErrors(propertyName, errors);
        }
    }
}
