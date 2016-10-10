// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RobotApp.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            this.SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            this.MyHamburgerMenu.NavigationService = navigationService;
        }
    }
}