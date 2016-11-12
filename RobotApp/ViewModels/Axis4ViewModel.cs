// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RobotApp.Services;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A view model for Axis4.
    /// </summary>
    [Subscription(publishingInterval: 500, keepAliveCount: 20)]
    public class Axis4ViewModel : ViewModelBase, IAxisViewModel
    {
        private PLC1Session session;
        private IDisposable subscriptionToken;

        public Axis4ViewModel(PLC1Session session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets the value of Axis.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Axis
        {
            get { return this.axis; }
            private set { this.Set(ref this.axis, value); }
        }

        private float axis;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        public string DisplayName => "Axis 4";

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            this.subscriptionToken = this.session?.Subscribe(this);
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            this.subscriptionToken?.Dispose();
            return base.OnNavigatedFromAsync(pageState, suspending);
        }
    }
}