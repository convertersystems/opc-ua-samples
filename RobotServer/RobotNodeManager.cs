/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;

namespace RobotServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class RobotNodeManager : CustomNodeManager2
    {
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public RobotNodeManager(IServerInternal server, ApplicationConfiguration configuration)
            : base(server, configuration, Namespaces.RobotServer)
        {
            SystemContext.NodeIdFactory = this;
        }

        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                string id = instance.Parent.NodeId.Identifier as string;

                if (id != null)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
        }

        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                this.robot1State = CreateFolder(null, "Robot1", "Robot1");
                robot1State.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, robot1State.NodeId));
                robot1State.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(robot1State);

                try
                {
                    this.axis1State = CreateVariable(robot1State, "Robot1_Axis1", "Axis1", DataTypeIds.Float, ValueRanks.Scalar, 0f);
                    this.axis2State = CreateVariable(robot1State, "Robot1_Axis2", "Axis2", DataTypeIds.Float, ValueRanks.Scalar, 0f);
                    this.axis3State = CreateVariable(robot1State, "Robot1_Axis3", "Axis3", DataTypeIds.Float, ValueRanks.Scalar, 0f);
                    this.axis4State = CreateVariable(robot1State, "Robot1_Axis4", "Axis4", DataTypeIds.Float, ValueRanks.Scalar, 0f);
                    this.modeState = CreateVariable(robot1State, "Robot1_Mode", "Mode", DataTypeIds.Int16, ValueRanks.Scalar, (short)2);
                    this.speedState = CreateVariable(robot1State, "Robot1_Speed", "Speed", DataTypeIds.Int16, ValueRanks.Scalar, (short)0);
                    this.laserState = CreateVariable(robot1State, "Robot1_Laser", "Laser", DataTypeIds.Boolean, ValueRanks.Scalar, false);

                    MethodState stopMethod = CreateMethod(robot1State, "Robot1_Stop", "Stop");
                    stopMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnStopCall);

                    MethodState multiplyMethod = CreateMethod(robot1State, "Robot1_Multiply", "Multiply");
                    // set input arguments
                    multiplyMethod.InputArguments = new PropertyState<Argument[]>(multiplyMethod);
                    multiplyMethod.InputArguments.NodeId = new NodeId(multiplyMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    multiplyMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    multiplyMethod.InputArguments.DisplayName = multiplyMethod.InputArguments.BrowseName.Name;
                    multiplyMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    multiplyMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    multiplyMethod.InputArguments.DataType = DataTypeIds.Argument;
                    multiplyMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    multiplyMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "a", Description = "A",  DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "b", Description = "B",  DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    multiplyMethod.OutputArguments = new PropertyState<Argument[]>(multiplyMethod);
                    multiplyMethod.OutputArguments.NodeId = new NodeId(multiplyMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    multiplyMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    multiplyMethod.OutputArguments.DisplayName = multiplyMethod.OutputArguments.BrowseName.Name;
                    multiplyMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    multiplyMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    multiplyMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    multiplyMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    multiplyMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "result", Description = "Result",  DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar }
                    };

                    multiplyMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnMultiplyCall);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }

                AddPredefinedNode(SystemContext, robot1State);
                simulationTimer = new Timer(DoSimulation, null, 250, 250);
            }
        }

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                this.simulationTimer.Dispose();
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
           ServerSystemContext context,
           NodeHandle handle,
           IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // TBD

            return null;
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = new NodeId(path, NamespaceIndex);
            folder.BrowseName = new QualifiedName(path, NamespaceIndex);
            folder.DisplayName = new LocalizedText("en", name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        private BaseObjectState CreateObject(NodeState parent, string path, string name)
        {
            BaseObjectState folder = new BaseObjectState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            folder.NodeId = new NodeId(path, NamespaceIndex);
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        /// <summary>
        /// Creates a new object type.
        /// </summary>
        private BaseObjectTypeState CreateObjectType(NodeState parent, IDictionary<NodeId, IList<IReference>> externalReferences, string path, string name)
        {
            BaseObjectTypeState type = new BaseObjectTypeState();

            type.SymbolicName = name;
            type.SuperTypeId = ObjectTypeIds.BaseObjectType;
            type.NodeId = new NodeId(path, NamespaceIndex);
            type.BrowseName = new QualifiedName(name, NamespaceIndex);
            type.DisplayName = type.BrowseName.Name;
            type.WriteMask = AttributeWriteMask.None;
            type.UserWriteMask = AttributeWriteMask.None;
            type.IsAbstract = false;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(ObjectTypeIds.BaseObjectType, out references))
            {
                externalReferences[ObjectTypeIds.BaseObjectType] = references = new List<IReference>();
            }

            references.Add(new NodeStateReference(ReferenceTypes.HasSubtype, false, type.NodeId));

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, type.NodeId);
                type.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, type);
            return type;
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, BuiltInType dataType, int valueRank, object initialValue = null)
        {
            return CreateVariable(parent, path, name, (uint)dataType, valueRank, initialValue);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank, object initialValue = null)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = initialValue;
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        /// <summary>
        /// Creates a new variable type.
        /// </summary>
        private BaseVariableTypeState CreateVariableType(NodeState parent, IDictionary<NodeId, IList<IReference>> externalReferences, string path, string name, BuiltInType dataType, int valueRank)
        {
            BaseDataVariableTypeState type = new BaseDataVariableTypeState();

            type.SymbolicName = name;
            type.SuperTypeId = VariableTypeIds.BaseDataVariableType;
            type.NodeId = new NodeId(path, NamespaceIndex);
            type.BrowseName = new QualifiedName(name, NamespaceIndex);
            type.DisplayName = type.BrowseName.Name;
            type.WriteMask = AttributeWriteMask.None;
            type.UserWriteMask = AttributeWriteMask.None;
            type.IsAbstract = false;
            type.DataType = (uint)dataType;
            type.ValueRank = valueRank;
            type.Value = null;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(VariableTypeIds.BaseDataVariableType, out references))
            {
                externalReferences[VariableTypeIds.BaseDataVariableType] = references = new List<IReference>();
            }

            references.Add(new NodeStateReference(ReferenceTypes.HasSubtype, false, type.NodeId));

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, type.NodeId);
                type.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, type);
            return type;
        }

        /// <summary>
        /// Creates a new data type.
        /// </summary>
        private DataTypeState CreateDataType(NodeState parent, IDictionary<NodeId, IList<IReference>> externalReferences, string path, string name)
        {
            DataTypeState type = new DataTypeState();

            type.SymbolicName = name;
            type.SuperTypeId = DataTypeIds.Structure;
            type.NodeId = new NodeId(path, NamespaceIndex);
            type.BrowseName = new QualifiedName(name, NamespaceIndex);
            type.DisplayName = type.BrowseName.Name;
            type.WriteMask = AttributeWriteMask.None;
            type.UserWriteMask = AttributeWriteMask.None;
            type.IsAbstract = false;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(DataTypeIds.Structure, out references))
            {
                externalReferences[DataTypeIds.Structure] = references = new List<IReference>();
            }

            references.Add(new NodeStateReference(ReferenceTypeIds.HasSubtype, false, type.NodeId));

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, type.NodeId);
                type.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, type);
            return type;
        }

        /// <summary>
        /// Creates a new reference type.
        /// </summary>
        private ReferenceTypeState CreateReferenceType(NodeState parent, IDictionary<NodeId, IList<IReference>> externalReferences, string path, string name)
        {
            ReferenceTypeState type = new ReferenceTypeState();

            type.SymbolicName = name;
            type.SuperTypeId = ReferenceTypeIds.NonHierarchicalReferences;
            type.NodeId = new NodeId(path, NamespaceIndex);
            type.BrowseName = new QualifiedName(name, NamespaceIndex);
            type.DisplayName = type.BrowseName.Name;
            type.WriteMask = AttributeWriteMask.None;
            type.UserWriteMask = AttributeWriteMask.None;
            type.IsAbstract = false;
            type.Symmetric = true;
            type.InverseName = name;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(ReferenceTypeIds.NonHierarchicalReferences, out references))
            {
                externalReferences[ReferenceTypeIds.NonHierarchicalReferences] = references = new List<IReference>();
            }

            references.Add(new NodeStateReference(ReferenceTypeIds.HasSubtype, false, type.NodeId));

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, type.NodeId);
                type.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, type);
            return type;
        }

        /// <summary>
        /// Creates a new view.
        /// </summary>
        private ViewState CreateView(NodeState parent, IDictionary<NodeId, IList<IReference>> externalReferences, string path, string name)
        {
            ViewState type = new ViewState();

            type.SymbolicName = name;
            type.NodeId = new NodeId(path, NamespaceIndex);
            type.BrowseName = new QualifiedName(name, NamespaceIndex);
            type.DisplayName = type.BrowseName.Name;
            type.WriteMask = AttributeWriteMask.None;
            type.UserWriteMask = AttributeWriteMask.None;
            type.ContainsNoLoops = true;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(ObjectIds.ViewsFolder, out references))
            {
                externalReferences[ObjectIds.ViewsFolder] = references = new List<IReference>();
            }

            type.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ViewsFolder);
            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, type.NodeId));

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, type.NodeId);
                type.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, type);
            return type;
        }

        /// <summary>
        /// Creates a new method.
        /// </summary>
        private MethodState CreateMethod(NodeState parent, string path, string name)
        {
            MethodState method = new MethodState(parent);

            method.SymbolicName = name;
            method.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            method.NodeId = new NodeId(path, NamespaceIndex);
            method.BrowseName = new QualifiedName(path, NamespaceIndex);
            method.DisplayName = new LocalizedText("en", name);
            method.WriteMask = AttributeWriteMask.None;
            method.UserWriteMask = AttributeWriteMask.None;
            method.Executable = true;
            method.UserExecutable = true;

            if (parent != null)
            {
                parent.AddChild(method);
            }

            return method;
        }

        private ServiceResult OnStopCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            this.modeState.Value = (short)0;
            this.modeState.Timestamp = DateTime.Now;
            this.modeState.ClearChangeMasks(SystemContext, false);
            return ServiceResult.Good;
        }

        private ServiceResult OnMultiplyCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var a = (Double)inputArguments[0];
                var b = (Double)inputArguments[1];

                // set output parameter
                outputArguments[0] = (Double)(a * b);
                return ServiceResult.Good;
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                    var now = DateTime.Now;
                    double dt = (now - timestamp).Milliseconds;
                    timestamp = now;

                    var mode = (short)modeState.Value;
                    if (mode != 1) // if not in man mode
                    {
                        double period = 30000;
                        switch ((short)speedState.Value)
                        {
                            case 1:
                                period = 20000;
                                break;
                            case 2:
                                period = 10000;
                                break;
                            case 3:
                                period = 5000;
                                break;
                            default:
                                period = 30000;
                                break;
                        }
                        if (mode == 2) // auto mode
                        {
                            masterAxis = (masterAxis + dt / period) % 1.0; // 0.0 to 1.0
                        }
                        axis1State.Value = (float)(Math.Sin(masterAxis * 2.0 * Math.PI) * 45.0);
                        axis1State.Timestamp = now;
                        axis1State.ClearChangeMasks(SystemContext, false);
                        axis2State.Value = (float)(Math.Cos(masterAxis * 2.0 * Math.PI) * 45.0);
                        axis2State.Timestamp = now;
                        axis2State.ClearChangeMasks(SystemContext, false);
                        axis3State.Value = (float)(Math.Sin(((masterAxis * 2.0) % 1.0) * 2.0 * Math.PI) * 45.0);
                        axis3State.Timestamp = now;
                        axis3State.ClearChangeMasks(SystemContext, false);
                        axis4State.Value = (float)(Math.Cos(masterAxis * 2.0 * Math.PI) * -180.0);
                        axis4State.Timestamp = now;
                        axis4State.ClearChangeMasks(SystemContext, false);

                    }

                    if (mode != prevmode)
                    {
                        prevmode = mode;

                        var e = new SystemEventState(null);
                        switch (mode)
                        {
                            case 1:
                                e.Initialize(SystemContext, this.robot1State, EventSeverity.Medium, new LocalizedText("Mode to Hand"));
                                break;
                            case 2:
                                e.Initialize(SystemContext, this.robot1State, EventSeverity.Medium, new LocalizedText("Mode to Auto"));
                                break;
                            default:
                                e.Initialize(SystemContext, this.robot1State, EventSeverity.Medium, new LocalizedText("Mode to Off"));
                                break;
                        }
                        this.robot1State.ReportEvent(SystemContext, e);
                    }

                    var laser = (bool)this.laserState.Value;
                    if (laser != prevlaser)
                    {
                        prevlaser = laser;

                        var e = new SystemEventState(null);
                        e.Initialize(SystemContext, this.robot1State, EventSeverity.Medium, new LocalizedText(laser ? "Laser activated." : "Laser deactivated."));
                        this.robot1State.ReportEvent(SystemContext, e);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error doing simulation.");
            }
        }

        private Timer simulationTimer;
        private FolderState robot1State;
        private BaseDataVariableState axis1State;
        private BaseDataVariableState axis2State;
        private BaseDataVariableState axis3State;
        private BaseDataVariableState axis4State;
        private BaseDataVariableState modeState;
        private BaseDataVariableState speedState;
        private BaseDataVariableState laserState;
        private double masterAxis;
        private DateTime timestamp = DateTime.MinValue;
        private short prevmode = 2;
        private bool prevlaser = false;
    }
}
