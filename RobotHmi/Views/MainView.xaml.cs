// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Regions;

namespace RobotHmi.Views
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public sealed partial class MainView : IRegionMemberLifetime
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets a value indicating whether this view and viewmodel are kept alive while in the navigation history.
        /// </summary>
        public bool KeepAlive => true;
    }
}