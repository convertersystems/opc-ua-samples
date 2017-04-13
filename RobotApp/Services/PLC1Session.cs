// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Extensions.Logging;
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
        public PLC1Session(ILoggerFactory loggerFactory)
            : base(
                  new ApplicationDescription
                  {
                      ApplicationName = "Workstation.RobotApp",
                      ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotApp",
                      ApplicationType = ApplicationType.Client
                  },
                new DirectoryStore(
                    Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "pki"),
                    loggerFactory: loggerFactory),
                  App.Current.ProvideUserIdentity,
                  SettingsService.Instance.EndpointUrl,
                  loggerFactory)
        {
        }
    }
}
