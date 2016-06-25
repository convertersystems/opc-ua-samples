// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.ServiceModel.Ua;

namespace RobotHmi.Services
{
    /// <summary>
    /// This application's description provided to Opc UA servers.
    /// </summary>
    public class AppDescription : ApplicationDescription
    {
        public AppDescription()
        {
            this.ApplicationName = "Workstation.RobotHmi";
            this.ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotHmi";
            this.ApplicationType = ApplicationType.Client;
        }
    }
}