// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;
using Workstation.ServiceModel.Ua;

namespace RobotApp.Services
{
    /// <summary>
    /// A service for communicating with PLC1's OPC UA server.
    /// </summary>
    public class PLC1Service : UaTcpSessionClient
    {
        public PLC1Service(ApplicationDescription description, X509Certificate2 certificate, IUserIdentity userIdentity, string discoveryUrl)
            : base(description, certificate, userIdentity, discoveryUrl)
        {
        }
    }
}