// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Regions;
using RobotHmi.Services;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for DetailView.
    /// </summary>
    public class DetailViewModel : Subscription
    {
        public DetailViewModel(PLC1Service session)
            : base(session, publishingInterval: 250.0, keepAliveCount: 40)
        {
        }

        // add properties here

    }
}