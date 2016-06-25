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
    public class DetailViewModel : NavigationAwareSubscriptionBase
    {
        public DetailViewModel(PLC1Service service)
            : base(service)
        {
        }

        // add properties here

    }
}