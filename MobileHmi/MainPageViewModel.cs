// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Workstation.ServiceModel.Ua;

namespace Workstation.MobileHmi
{
    /// <summary>
    /// A model for MainView.
    /// </summary>
    [Subscription(endpointName: "PLC1", publishingInterval: 500, keepAliveCount: 20)]
    public class MainPageViewModel : SubscriptionBase
    {
        /// <summary>
        /// Gets or sets the value of Robot1Mode.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Mode")]
        public short Robot1Mode
        {
            get { return this.robot1Mode; }
            set { this.SetValue(ref this.robot1Mode, value); }
        }

        private short robot1Mode;

        /// <summary>
        /// Gets or sets the value of Robot1Axis1.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1")]
        public float Robot1Axis1
        {
            get { return this.robot1Axis1; }
            set { this.SetValueWithDeadband(ref this.robot1Axis1, value, 1.0f); }
        }

        private float robot1Axis1;

        /// <summary>
        /// Gets or sets the value of Robot1Axis2.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Robot1Axis2
        {
            get { return this.robot1Axis2; }
            set { this.SetValueWithDeadband(ref this.robot1Axis2, value, 1.0f); }
        }

        private float robot1Axis2;

        /// <summary>
        /// Gets or sets the value of Robot1Axis3.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3")]
        public float Robot1Axis3
        {
            get { return this.robot1Axis3; }
            set { this.SetValueWithDeadband(ref this.robot1Axis3, value, 1.0f); }
        }

        private float robot1Axis3;

        /// <summary>
        /// Gets or sets the value of Robot1Axis4.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Robot1Axis4
        {
            get { return this.robot1Axis4; }
            set { this.SetValueWithDeadband(ref this.robot1Axis4, value, 1.0f); }
        }

        private float robot1Axis4;

        /// <summary>
        /// Added to help with Xamarin Forms implementation of Slider.
        /// </summary>
        protected virtual bool SetValueWithDeadband(ref float storage, float value, float deadband, [CallerMemberName] string propertyName = null)
        {
            if (Math.Abs(storage - value) < deadband)
            {
                return false;
            }

            storage = value;
            this.NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
