// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Regions;

namespace RobotHmi.Views
{
    /// <summary>
    /// A view for an Axis.
    /// </summary>
    public sealed partial class AxisView : IRegionMemberLifetime
    {
        public AxisView()
        {
            this.InitializeComponent();
        }

        // Indicates whether this view and viewmodel are kept alive while in the navigation history.
        public bool KeepAlive
        {
            get
            {
                return false;
            }
        }
    }
}