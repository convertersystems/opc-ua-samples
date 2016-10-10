// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;

// Step 1: Add the following namespaces.
using Workstation.Collections;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A type of switch with three positions
    /// </summary>
    public enum HandOffAuto : short
    {
        Off = 0,
        Hand = 1,
        Auto = 2
    }

    /// <summary>
    /// A view model for MainView.
    /// </summary>
    [Subscription(publishingInterval: 250, keepAliveCount: 40)] // Step 2: Add a [Subscription] attribute.
    public class MainViewModel : ViewModelBase // Step 3: Add your base class (which implements INotifyPropertyChanged).
    {
        /// <summary>
        /// Gets or sets the value of Robot1Mode.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Mode")] // Step 4: Add a [MonitoredItem] attribute.
        public short Robot1Mode
        {
            get { return this.robot1Mode; }
            set { this.SetProperty(ref this.robot1Mode, value); }
        }

        private short robot1Mode;

        /// <summary>
        /// Gets or sets the value of Robot1Axis1.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1")]
        public float Robot1Axis1
        {
            get { return this.robot1Axis1; }
            set { this.SetProperty(ref this.robot1Axis1, value); }
        }

        private float robot1Axis1;

        /// <summary>
        /// Gets or sets the value of Robot1Axis2.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis2")]
        public float Robot1Axis2
        {
            get { return this.robot1Axis2; }
            set { this.SetProperty(ref this.robot1Axis2, value); }
        }

        private float robot1Axis2;

        /// <summary>
        /// Gets or sets the value of Robot1Axis3.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis3")]
        public float Robot1Axis3
        {
            get { return this.robot1Axis3; }
            set { this.SetProperty(ref this.robot1Axis3, value); }
        }

        private float robot1Axis3;

        /// <summary>
        /// Gets or sets the value of Robot1Axis4.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis4")]
        public float Robot1Axis4
        {
            get { return this.robot1Axis4; }
            set { this.SetProperty(ref this.robot1Axis4, value); }
        }

        private float robot1Axis4;

        /// <summary>
        /// Gets or sets the value of Robot1Speed.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Speed")]
        public short Robot1Speed
        {
            get { return this.robot1Speed; }
            set { this.SetProperty(ref this.robot1Speed, value); }
        }

        private short robot1Speed;

        /// <summary>
        /// Gets or sets a value indicating whether Robot1Laser is active.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Laser")]
        public bool Robot1Laser
        {
            get { return this.robot1Laser; }
            set { this.SetProperty(ref this.robot1Laser, value); }
        }

        private bool robot1Laser;

        /// <summary>
        /// Gets the recent history of the value of Robot1Axis1.
        /// </summary>
        /// <remarks>
        /// capacity: 240 = 60 seconds storage /  0.250 seconds publishing interval
        /// isFixedSize: true = circular queue, oldest values are overwitten
        /// </remarks>
        [MonitoredItem(nodeId: "ns=2;s=Robot1_Axis1", dataChangeTrigger: DataChangeTrigger.StatusValueTimestamp)]
        public ObservableQueue<DataValue> Robot1Axis1Queue { get; } = new ObservableQueue<DataValue>(capacity: 240, isFixedSize: true);

        /// <summary>
        /// Gets the events of Robot1.
        /// </summary>
        [MonitoredItem(nodeId: "ns=2;s=Robot1", attributeId: AttributeIds.EventNotifier)]
        public ObservableQueue<AlarmCondition> Robot1Events { get; } = new ObservableQueue<AlarmCondition>(capacity: 16, isFixedSize: true);

        /// <summary>
        /// Gets the command to set the value of Robot1Mode to Off.
        /// </summary>
        public ICommand Robot1ModeOffCommand
        {
            get
            {
                return DelegateCommand.FromAsyncHandler(async () =>
                {
                    try
                    {
                        await UaTcpSessionClient.FromModel(this).WriteAsync(new WriteRequest
                        {
                            NodesToWrite = new[]
                            {
                                new WriteValue
                                {
                                    NodeId = NodeId.Parse("ns=2;s=Robot1_Mode"),
                                    AttributeId = AttributeIds.Value,
                                    IndexRange = null,
                                    Value = new DataValue((short)HandOffAuto.Off)
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error writing to NodeId {0} : {1}", "ns=2;s=Robot1_Mode", ex);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the command to set the value of Robot1Mode to Auto.
        /// </summary>
        public ICommand Robot1ModeAutoCommand
        {
            get
            {
                return DelegateCommand.FromAsyncHandler(async () =>
                {
                    try
                    {
                        await UaTcpSessionClient.FromModel(this).WriteAsync(new WriteRequest
                        {
                            NodesToWrite = new[]
                            {
                                new WriteValue
                                {
                                    NodeId = NodeId.Parse("ns=2;s=Robot1_Mode"),
                                    AttributeId = AttributeIds.Value,
                                    IndexRange = null,
                                    Value = new DataValue((short)HandOffAuto.Auto)
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error writing to NodeId {0} : {1}", "ns=2;s=Robot1_Mode", ex);
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets the value of InputA.
        /// </summary>
        public double InputA
        {
            get { return this.inputA; }
            set { this.SetProperty(ref this.inputA, value); }
        }

        private double inputA;

        /// <summary>
        /// Gets or sets the value of InputB.
        /// </summary>
        public double InputB
        {
            get { return this.inputB; }
            set { this.SetProperty(ref this.inputB, value); }
        }

        private double inputB;

        /// <summary>
        /// Gets or sets the value of Result.
        /// </summary>
        public double Result
        {
            get { return this.result; }
            set { this.SetProperty(ref this.result, value); }
        }

        private double result;

        /// <summary>
        /// Gets the command to call the method named Robot1Multiply.
        /// </summary>
        public ICommand Robot1MultiplyCommand
        {
            get
            {
                return DelegateCommand.FromAsyncHandler(async () =>
                {
                    try
                    {
                        // Call the method, passing the input arguments in a Variant[].
                        var response = await UaTcpSessionClient.FromModel(this).CallAsync(new CallRequest
                        {
                            MethodsToCall = new[]
                            {
                                new CallMethodRequest
                                {
                                    ObjectId = NodeId.Parse("ns=2;s=Robot1"),
                                    MethodId = NodeId.Parse("ns=2;s=Robot1_Multiply"),
                                    InputArguments = new Variant[] { this.InputA, this.InputB }
                                }
                            }
                        });

                        // When the method returns, cast the output arguments.
                        this.Result = (double)response.Results[0].OutputArguments[0];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error calling Robot1Multiply method: {0}", ex);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the command to clear the form.
        /// </summary>
        public ICommand ClearCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.InputA = 0d;
                    this.InputB = 0d;
                    this.Result = 0d;
                    GC.Collect();
                });
            }
        }
    }
}