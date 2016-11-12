// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using RobotApp.Services.SettingsServices;
using Workstation.ServiceModel.Ua;

namespace RobotApp.Services
{
    /// <summary>
    /// A client for browsing, reading, writing and subscribing to nodes of the OPC UA server for PLC1.
    /// </summary>
    public class PLC1Session : UaTcpSessionClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PLC1Session"/> class.
        /// </summary>
        /// <param name="appDescription"></param>
        /// <param name="appCertificate"></param>
        /// <param name="userIdentityProvider"></param>
        public PLC1Session(ApplicationDescription appDescription, X509Certificate2 appCertificate, Func<EndpointDescription, Task<IUserIdentity>> userIdentityProvider)
            : base(appDescription, appCertificate, userIdentityProvider, SettingsService.Instance.EndpointUrl)
        {

        }
    }
}
