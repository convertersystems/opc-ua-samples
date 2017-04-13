// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Workstation.ServiceModel.Ua;

namespace Workstation.MobileHmi
{
    /// <summary>
    /// A model for MainView.
    /// </summary>
    [Subscription(publishingInterval: 500, keepAliveCount: 20)]
    public class MainPageViewModel : ViewModelBase
    {
        private readonly UaTcpSessionClient session;

        public MainPageViewModel(UaTcpSessionClient session)
        {
            this.session = session;
            if (session != null)
            {
                session.Subscribe(this);

                // Update UI when connection state changes
                session.StateChanged += (s, e) =>
                {
                    this.NotifyPropertyChanged(nameof(this.IsDisconnected));
                };
            }

            // Update UI when Item has errors. This is not necessary with WPF.
            this.ErrorsChanged += (s, e) =>
            {
                this.NotifyPropertyChanged(nameof(this.Robot1ModeHasError));
                this.NotifyPropertyChanged(nameof(this.Robot1ModeError));
            };
        }

        /// <summary>
        /// Gets a value indicating whether the channel is disconnected.
        /// </summary>
        public bool IsDisconnected
        {
            get { return this.session.State != CommunicationState.Opened; }
        }

        /// <summary>
        /// Gets or sets the value of Robot1Mode.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Mode")]
        public short Robot1Mode
        {
            get { return this.robot1Mode; }
            set { this.SetProperty(ref this.robot1Mode, value); }
        }

        private short robot1Mode;

        /// <summary>
        /// Gets a value indicating whether Robot1Mode has OPCUA errors. This is not necessary with WPF.
        /// </summary>
        public bool Robot1ModeHasError => this.GetErrors(nameof(this.Robot1Mode)).OfType<string>().Any();

        /// <summary>
        /// Gets the first OPCUA error of Robot1Mode. This is not necessary with WPF.
        /// </summary>
        public string Robot1ModeError => this.GetErrors(nameof(this.Robot1Mode)).OfType<string>().FirstOrDefault();

        /// <summary>
        /// Gets or sets the value of Robot1Axis1.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1")]
        public float Robot1Axis1
        {
            get { return this.robot1Axis1; }
            set { this.SetProperty(ref this.robot1Axis1, value, 1.0f); }
        }

        private float robot1Axis1;

        /// <summary>
        /// Gets or sets the value of Robot1Axis2.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Robot1Axis2
        {
            get { return this.robot1Axis2; }
            set { this.SetProperty(ref this.robot1Axis2, value, 1.0f); }
        }

        private float robot1Axis2;

        /// <summary>
        /// Gets or sets the value of Robot1Axis3.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3")]
        public float Robot1Axis3
        {
            get { return this.robot1Axis3; }
            set { this.SetProperty(ref this.robot1Axis3, value, 1.0f); }
        }

        private float robot1Axis3;

        /// <summary>
        /// Gets or sets the value of Robot1Axis4.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Robot1Axis4
        {
            get { return this.robot1Axis4; }
            set { this.SetProperty(ref this.robot1Axis4, value, 1.0f); }
        }

        private float robot1Axis4;
    }

    internal class MainViewModelDesignInstance : MainPageViewModel
    {
        public MainViewModelDesignInstance()
            : base(null)
        {
        }
    }
}
