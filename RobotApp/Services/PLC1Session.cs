// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public PLC1Session()
            : base(
                  App.Current.ApplicationDescription,
                  App.Current.ProvideApplicationCertificate,
                  App.Current.ProvideUserIdentity,
                  SettingsService.Instance.EndpointUrl)
        {
        }
    }
}
