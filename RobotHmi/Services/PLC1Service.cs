// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.ServiceModel.Ua;

namespace RobotHmi.Services
{
    /// <summary>
    /// A service for communicating with PLC1 opc ua server.
    /// </summary>
    public class PLC1Service : UaTcpSessionService
    {
        public PLC1Service(AppDescription description)
            : base(description, description.GetCertificate(), null, "opc.tcp://localhost:26543")
        {
        }
    }
}